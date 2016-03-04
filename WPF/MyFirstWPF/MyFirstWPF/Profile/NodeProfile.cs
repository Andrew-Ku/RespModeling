using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using MyFirstWPF.Models;

namespace MyFirstWPF.Profile
{
    public class NodeProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Node, Node>();

            Mapper.CreateMap<NodeVm, NodeVm>();

            Mapper.CreateMap<TextBlock, TextBlockSave>();

            Mapper.CreateMap<ArrowLine, ArrowLineSave>();

        }
    }
}
