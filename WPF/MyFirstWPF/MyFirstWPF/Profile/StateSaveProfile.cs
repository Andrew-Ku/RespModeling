using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AutoMapper;
using MyFirstWPF.Models;

namespace MyFirstWPF.Profile
{
    class StateSaveProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            ModeStateSaveMap();
            NodeSaveMap();
            EdgeSaveMap();
        }

        private void ModeStateSaveMap()
        {
            //Mapper.CreateMap<NodeSave, ModelStateSave>();
            //Mapper.CreateMap<EdgeSave, ModelStateSave>();

            //Mapper.CreateMap<ModelStateSave, EdgeSave>();
            //Mapper.CreateMap<ModelStateSave, NodeSave>();
        }

        private void NodeSaveMap()
        {
            Mapper.CreateMap<Node, NodeSave>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.IsRejectionNode, o => o.MapFrom(s => s.IsRejectionNode))
                .ForMember(d => d.NodeRelations, o => o.MapFrom(s => s.NodeRelations))
                .ForMember(d => d.IsStartNode, o => o.MapFrom(s => s.IsStartNode));

            Mapper.CreateMap<NodeVm, NodeSave>()
                 .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Position, o => o.MapFrom(s => s.Position))
                .ForMember(d => d.TextBlockName, o => o.MapFrom(s => s.TextBlock.Name))
                .ForMember(d => d.TextBlockText, o => o.MapFrom(s => s.TextBlock.Text));

            Mapper.CreateMap<NodeSave, Node>();
            Mapper.CreateMap<NodeSave, NodeVm>();
         
        }

        private void EdgeSaveMap()
        {
            Mapper.CreateMap<EdgeVm, EdgeSave>()
                .ForMember(d => d.ArrowEnds, o => o.MapFrom(s => s.ArrowLine.ArrowEnds))
                .ForMember(d => d.FromNodeVmId, o => o.MapFrom(s => s.FromNodeVmId))
                .ForMember(d => d.ToNodeVmId, o => o.MapFrom(s => s.ToNodeVmId))
                .ForMember(d => d.X1, o => o.MapFrom(s => s.ArrowLine.X1))
                .ForMember(d => d.X2, o => o.MapFrom(s => s.ArrowLine.X2))
                .ForMember(d => d.Y1, o => o.MapFrom(s => s.ArrowLine.Y1))
                .ForMember(d => d.Y2, o => o.MapFrom(s => s.ArrowLine.Y2));

            Mapper.CreateMap<EdgeSave, EdgeVm>()
                .ForMember(d => d.FromNodeVmId, o => o.MapFrom(s => s.FromNodeVmId))
                .ForMember(d => d.ToNodeVmId, o => o.MapFrom(s => s.ToNodeVmId));

            Mapper.CreateMap<EdgeSave, ArrowLine>()
                .ForMember(d => d.ArrowEnds, o => o.MapFrom(s => s.ArrowEnds))
                .ForMember(d => d.X1, o => o.MapFrom(s => s.X1))
                .ForMember(d => d.X2, o => o.MapFrom(s => s.X2))
                .ForMember(d => d.Y1, o => o.MapFrom(s => s.Y1))
                .ForMember(d => d.Y2, o => o.MapFrom(s => s.Y2));
        }
    }
}
