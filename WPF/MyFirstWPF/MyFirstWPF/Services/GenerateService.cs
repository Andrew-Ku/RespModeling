﻿using System;
using System.Collections.Generic;
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
        public static void GenerateGpssFile(GpssInputModel model, string path)
        {
            var result = new StringBuilder();
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


            File.WriteAllText(path, result.ToString());
        
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
                    result.AppendLine(string.Format("Lambda{0}{1} EQU {2}", node.Id, relation.NodeId, relation.Weight.ToString().Replace(",",".")));
                }
            }

            result.AppendLine("");

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
            }

            result.AppendLine("");

            foreach (var node in model.Nodes)
            {
                foreach (var relation in node.NodeRelations)
                {
                    result.AppendLine(string.Format("initial x$LambdaTime{0}{1},0 ; время по интенсивности", node.Id, relation.NodeId));
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
                result.AppendLine(string.Format("CorrectState{0}Met SAVEVALUE StateTime{0}+,(Time - ({1}))", node.Id,sumString));
                result.AppendLine(string.Format("TRANSFER ,FinishMet"));
            }
         
            result.AppendLine(";---------------------------------------------------------------");

            return result.ToString();
        }

        private static string GetStateBlock(Node node)
        {
            var result = new StringBuilder();
            result.AppendLine(string.Format(";--Состояние {0}-----------------------", node.Id));
            result.AppendLine(string.Format("State{0}Met ASSEMBLE 1", node.Id));

            foreach (var relation in node.NodeRelations)
            {
                result.AppendLine(string.Format("SAVEVALUE LambdaTime{0}{1},(Exponential(1,0,1/Lambda{0}{1}))", node.Id, relation.NodeId));
            }

            var firstRelation = node.NodeRelations.FirstOrDefault();
            var relationCount = node.NodeRelations.Count();

            if (firstRelation != null)
            {
                result.AppendLine(string.Format("ASSIGN State,State{0}Met", firstRelation.NodeId));
                result.AppendLine(string.Format("ASSIGN Time,x$LambdaTime{0}{1}", node.Id, firstRelation.NodeId));
            }
           
            if (node.IsRejectionNode)
            {
                result.AppendLine(string.Format("ASSIGN ReturnState,ReturnState{0}Met", node.Id));
            }

            if (relationCount > 1)
            {
                for (var i = 1; i < relationCount; i++)
                {
                    var relation = node.NodeRelations.ElementAt(i);

                    if (i != relationCount - 1)
                    {
                        var nextRelation = node.NodeRelations.ElementAt(i + 1);

                        result.AppendLine(string.Format("Advance{0}{1}Met TEST L x$LambdaTime{0}{1},p$Time,Advance{0}{2}",node.Id, relation.NodeId, nextRelation));
                        result.AppendLine(string.Format("ASSIGN State,State{0}Met", relation.NodeId));
                        result.AppendLine(string.Format("ASSIGN Time,x$LambdaTime{0}{1}", node.Id, relation.NodeId));
                    }
                    else
                    {
                        result.AppendLine(string.Format("Advance{0}{1}Met TEST L x$LambdaTime{0}{1},p$Time,exAdvance{0}Met", node.Id, relation.NodeId));
                        result.AppendLine(string.Format("ASSIGN State,State{0}Met", relation.NodeId));
                        result.AppendLine(string.Format("ASSIGN Time,x$LambdaTime{0}{1}", node.Id, relation.NodeId));
                    }
                }
            }

            result.AppendLine(string.Format("exAdvance{0}Met SAVEVALUE CorrectStateMet,CorrectState{0}Met", node.Id));
            result.AppendLine(string.Format("SAVEVALUE IsCorrect,True"));

            result.AppendLine(node.IsRejectionNode ? string.Format("TRANSFER ,DevMet") : string.Format("ADVANCE p$Time"));
            result.AppendLine(node.IsRejectionNode ? string.Format("ReturnState{0}Met SAVEVALUE IsCorrect,False",node.Id) : string.Format("SAVEVALUE IsCorrect,False"));
            result.AppendLine(string.Format("SAVEVALUE StateTime{0}+,p$Time", node.Id));
            
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

            foreach (var node in model.Nodes.Where(n=>!n.IsRejectionNode))
            {
                sumWork.Append(string.Format("x$StateTime{0}+", node.Id));
            }
            sumWork.Remove(sumWork.Length - 1, 1);

            foreach (var node in model.Nodes.Where(n=>n.IsRejectionNode))
            {
                sumNotWork.Append(string.Format("x$StateTime{0}+", node.Id));
            }
            sumNotWork.Remove(sumNotWork.Length - 1, 1);

            result.AppendLine(";--Конечный блок--------------------");
            result.AppendLine(string.Format("GENERATE Time"));
            result.AppendLine(string.Format("TEST E x$IsCorrect,True,FinishMet"));
            result.AppendLine(string.Format("TRANSFER ,x$CorrectStateMet"));
            result.AppendLine(string.Format("FinishMet SAVEVALUE TimeAll,({0})", sumString));
            result.AppendLine(string.Format("SAVEVALUE notWorkTimeAll,({0})", sumNotWork));
            result.AppendLine(string.Format("SAVEVALUE workTimeAll,({0})", sumWork));

            foreach (var node in model.Nodes)
            {
                result.AppendLine(string.Format("SAVEVALUE prob{0},(x$StateTime{0}/x$TimeAll)", node.Id));
            }

            result.AppendLine(string.Format("SAVEVALUE kGotov,(x$workTimeAll/(x$notWorkTimeAll+x$workTimeAll))"));
            result.AppendLine(string.Format("TERMINATE 1 "));

            result.AppendLine(";--------------------------------------------------------------");

            return result.ToString();
        }
    }
}
