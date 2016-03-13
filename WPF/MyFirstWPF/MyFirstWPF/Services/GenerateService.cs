using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstWPF.Models;
using MyFirstWPF.Models.Generation;

namespace MyFirstWPF.Services
{
    public class GenerateService
    {
        private static int maxRelationCount;
        public static void GenerateGpssFile(GpssInputModel model, string path)
        {
            var result = new StringBuilder();
            maxRelationCount = model.Nodes.Max(n => n.NodeRelations.Count);

            result.Append(GetInitializationBlock(model));
            result.Append(GetInfoBlock());
            result.Append(GetFinishBlock(model));
            result.Append(GetStartBlock(model));

            foreach (var node in model.Nodes)
            {
                result.Append(GetStateBlock(node));
            }

            result.Append(GetDeviceBlock());
            result.Append(GetHelpfulBlock(model));
            result.Append(GetProcedureBlock(model));

            File.WriteAllText(path, result.ToString());

            Process.Start(path);

        }

        // Формируем блок инициализации
        private static string GetInitializationBlock(GpssInputModel model)
        {
            var result = new StringBuilder();
            result.AppendLine(";--Инициализирующий блок----------------");
            result.AppendLine(string.Format("Time EQU {0} ; время моелирования", model.ModelingTime));
            result.AppendLine("True EQU 1");
            result.AppendLine("False EQU 0");

            foreach (var node in model.Nodes)
            {
                foreach (var relation in node.NodeRelations)
                {
                    result.AppendLine(string.Format("Lambda{0}_{1} EQU {2}", node.Id, relation.NodeId, relation.Weight.ToString().Replace(",", ".")));
                }
            }

            result.AppendLine("");
            result.AppendLine("TimeStateMat MATRIX ,1,2; матрица для хранения времени и следующего состояния");
            result.AppendLine("initial x$workTimeAll,0");
            result.AppendLine("initial x$notWorkTimeAll,0");
            result.AppendLine("initial x$kGotov,0 ; коэффициент готовности");
            result.AppendLine("initial x$CorrectStateMet,0 ;Метка для коррекции");
            result.AppendLine("initial x$IsCorrect,0 ; нужна ли коррекция ");
            result.AppendLine("initial x$kGotov,0 ; коэффициент готовности");

            result.AppendLine("");
            foreach (var node in model.Nodes)
            {
                result.AppendLine(string.Format("initial x$StateTime{0},0 ; время в состояние {0}", node.Id));
                result.AppendLine(string.Format("initial x$StateCount{0},0 ; число заходов с состояние {0}", node.Id));
            }

            result.AppendLine("");

            foreach (var node in model.Nodes)
            {
                foreach (var relation in node.NodeRelations)
                {
                    result.AppendLine(string.Format("initial x$LambdaTime{0}_{1},0 ; время по интенсивности", node.Id, relation.NodeId));
                }
            }

            result.AppendLine(";---------------------------------------------------------------");

            return result.ToString();
        }

        private static string GetStartBlock(GpssInputModel model)
        {
            var result = new StringBuilder();
            result.AppendLine(";--Стартовый блок----------------");
            result.AppendLine("GENERATE	,,,1,");
            result.AppendLine("SEIZE DEV");
            result.AppendLine("RELEASE DEV");
            result.AppendLine(string.Format("TRANSFER ,State{0}Met", model.Nodes.Single(n => n.IsStartNode).Id));

            result.AppendLine(";---------------------------------------------------------------");

            return result.ToString();
        }
        private static string GetInfoBlock()
        {
            var result = new StringBuilder();
            result.AppendLine(";--Непонятный блок----------------");

            result.AppendLine("START 1");
            result.AppendLine("RMULT 1281");

            result.AppendLine(";---------------------------------------------------------------");

            return result.ToString();
        }

        private static string GetDeviceBlock()
        {
            var result = new StringBuilder();
            result.AppendLine(";--Устройство--------------------");
            result.AppendLine("DevMet SEIZE DEV");
            result.AppendLine("ADVANCE p$Time");
            result.AppendLine("RELEASE DEV");
            result.AppendLine("TRANSFER ,p$ReturnState");
            result.AppendLine(";---------------------------------------------------------------");

            return result.ToString();
        }

        private static string GetHelpfulBlock(GpssInputModel model)
        {
            var result = new StringBuilder();
            var sumString = new StringBuilder();

            result.AppendLine(";--Вспомогательный блок--------------------");

            foreach (var node in model.Nodes)
            {
                sumString.Append(string.Format("x$StateTime{0}+", node.Id));
            }
            sumString.Remove(sumString.Length - 1, 1);

            foreach (var node in model.Nodes)
            {
                result.AppendLine(string.Format("CorrectState{0}Met SAVEVALUE StateTime{0}+,(Time - ({1}))", node.Id, sumString));
                result.AppendLine(string.Format("TRANSFER ,FinishMet"));
            }

            result.AppendLine(";---------------------------------------------------------------");

            return result.ToString();
        }

        private static string GetStateBlock(Node node)
        {
            var result = new StringBuilder();
            result.AppendLine(string.Format(";--Состояние {0}-----------------------", node.Id));
          //  result.AppendLine(string.Format("State{0}Met ASSEMBLE 1", node.Id));

            foreach (var relation in node.NodeRelations)
            {
                if (relation.Equals(node.NodeRelations.First()))
                {
                    result.AppendLine(string.Format("State{0}Met SAVEVALUE LambdaTime{0}_{1},(Exponential(1,0,1/Lambda{0}_{1}))", node.Id, relation.NodeId));
                }
                else
                {
                    result.AppendLine(string.Format("SAVEVALUE LambdaTime{0}_{1},(Exponential(1,0,1/Lambda{0}_{1}))", node.Id, relation.NodeId));
                }
                
            }

            var firstRelation = node.NodeRelations.FirstOrDefault();
            var relationCount = node.NodeRelations.Count();

            //if (firstRelation != null)
            //{
            //    result.AppendLine(string.Format("ASSIGN State,State{0}Met", firstRelation.NodeId));
            //    result.AppendLine(string.Format("ASSIGN Time,x$LambdaTime{0}_{1}", node.Id, firstRelation.NodeId));
            //}

            if (node.IsRejectionNode)
            {
                result.AppendLine(string.Format("ASSIGN ReturnState,ReturnState{0}Met", node.Id));
            }


            var procArgs = new StringBuilder();


            for (var i = 0; i < maxRelationCount; i++)
            {
                if (i < relationCount)
                {
                    var relation = node.NodeRelations.ElementAt(i);
                    procArgs.Append(string.Format("x$LambdaTime{0}_{1},State{1}Met,", node.Id, relation.NodeId));
                }
                else
                {
                    procArgs.Append(string.Format("{0},{1},", 999999, 999999));
                }
            }


            result.AppendLine(string.Format("SAVEVALUE procHelpfulPar,(ChooseWayProc({0}))", procArgs.Remove(procArgs.Length - 1, 1)));
            result.AppendLine(string.Format("ASSIGN Time,MX$TimeStateMat(1,1)"));
            result.AppendLine(string.Format("ASSIGN State,MX$TimeStateMat(1,2)"));

            result.AppendLine(string.Format("SAVEVALUE CorrectStateMet,CorrectState{0}Met", node.Id));
           // result.AppendLine(string.Format("SAVEVALUE IsCorrect,True"));
            result.AppendLine(string.Format("SAVEVALUE StateCount{0}+,1", node.Id));

            result.AppendLine(node.IsRejectionNode ? string.Format("TRANSFER ,DevMet") : string.Format("ADVANCE p$Time"));
          //  result.AppendLine(node.IsRejectionNode ? string.Format("ReturnState{0}Met SAVEVALUE IsCorrect,False", node.Id) : string.Format("SAVEVALUE IsCorrect,False"));
            result.AppendLine(node.IsRejectionNode ? string.Format("ReturnState{0}Met SAVEVALUE StateTime{0}+,p$Time", node.Id) : string.Format("SAVEVALUE StateTime{0}+,p$Time",node.Id));
          
            result.AppendLine(string.Format("TRANSFER ,p$State"));
            result.AppendLine(";---------------------------------------------------------------");

            return result.ToString();
        }

        private static string GetFinishBlock(GpssInputModel model)
        {
            var result = new StringBuilder();
            var sumString = new StringBuilder();
            var sumWork = new StringBuilder();
            var sumNotWork = new StringBuilder();

            foreach (var node in model.Nodes)
            {
                sumString.Append(string.Format("x$StateTime{0}+", node.Id));
            }
            sumString.Remove(sumString.Length - 1, 1);

            foreach (var node in model.Nodes.Where(n => !n.IsRejectionNode))
            {
                sumWork.Append(string.Format("x$StateTime{0}+", node.Id));
            }
            sumWork.Remove(sumWork.Length - 1, 1);

            foreach (var node in model.Nodes.Where(n => n.IsRejectionNode))
            {
                sumNotWork.Append(string.Format("x$StateTime{0}+", node.Id));
            }
            sumNotWork.Remove(sumNotWork.Length - 1, 1);

            result.AppendLine(";--Конечный блок--------------------");
            result.AppendLine(string.Format("GENERATE Time"));
           // result.AppendLine(string.Format("TEST E x$IsCorrect,True,FinishMet"));
            result.AppendLine(string.Format("TRANSFER ,x$CorrectStateMet"));
            result.AppendLine(string.Format("FinishMet SAVEVALUE TimeAll,({0})", sumString));
            result.AppendLine(string.Format("SAVEVALUE notWorkTimeAll,({0})", sumNotWork));
            result.AppendLine(string.Format("SAVEVALUE workTimeAll,({0})", sumWork));

            foreach (var node in model.Nodes)
            {
                result.AppendLine(string.Format("SAVEVALUE prob{0},(x$StateTime{0}/x$TimeAll)", node.Id));
            }

            result.AppendLine(string.Format("SAVEVALUE kGotov,(x$workTimeAll/(x$notWorkTimeAll+x$workTimeAll))"));

            var sumCountWork = new StringBuilder();
            foreach (var node in model.Nodes.Where(n => !n.IsRejectionNode))
            {
                sumCountWork.Append(string.Format("x$StateCount{0}+", node.Id));
            }
            sumCountWork.Remove(sumCountWork.Length - 1, 1);

            var sumCountNotWork = new StringBuilder();
            foreach (var node in model.Nodes.Where(n => n.IsRejectionNode))
            {
                sumCountNotWork.Append(string.Format("x$StateCount{0}+", node.Id));
            }
            sumCountNotWork.Remove(sumCountNotWork.Length - 1, 1);

            result.AppendLine(string.Format("SAVEVALUE Tw,(x$workTimeAll/({0}))", sumCountWork));
            result.AppendLine(string.Format("SAVEVALUE Tv,(x$notWorkTimeAll/({0}))", sumCountNotWork));
            result.AppendLine(string.Format("SAVEVALUE kGotovMid,(x$Tw/(x$Tw+x$Tv))"));



            result.AppendLine(string.Format("TERMINATE 1 "));

            result.AppendLine(";--------------------------------------------------------------");

            return result.ToString();
        }

        private static string GetProcedureBlock(GpssInputModel model)
        {
            var result = new StringBuilder();
            result.AppendLine(";--Блок процедур----------------");
            result.AppendLine(";--Процедура для поиска слудующего состояния ----------------");

            var procArgs = new StringBuilder();
            for (var i = 0; i < maxRelationCount; i++)
            {
               procArgs.Append(string.Format("timeArg{0},nextStateArg{0},", i));
               
            }

            result.AppendLine(string.Format("PROCEDURE ChooseWayProc({0})", procArgs.Remove(procArgs.Length - 1, 1)));
            result.AppendLine(string.Format("BEGIN"));
            result.AppendLine(string.Format("TEMPORARY minTime,nextState;"));
            result.AppendLine(string.Format("minTime = timeArg0; nextState = nextStateArg0;"));

            for (var i = 1; i < maxRelationCount; i++)
            {
                result.AppendLine(string.Format("IF (minTime>timeArg{0}) THEN BEGIN minTime = timeArg{0}; nextState = nextStateArg{0}; END;", i));

            }
            result.AppendLine(string.Format("TimeStateMat[1,1] = minTime; TimeStateMat[1,2] = nextState;"));
            result.AppendLine(string.Format("Return (nextState);"));
            result.AppendLine(string.Format("END;"));
            result.AppendLine(";--------------------------------------------------------------");

            return result.ToString();
        }
        
    }
}
