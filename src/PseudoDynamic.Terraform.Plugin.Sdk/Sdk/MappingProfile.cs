﻿using AutoMapper;
using PseudoDynamic.Terraform.Plugin.Protocols.Consolidated;

namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    internal class MappingProfile : Profile
    {
        private static Diagnostic.DiagnosticSeverity ToDiagnosticSeverity(ReportKind reportKind) => reportKind switch {
            ReportKind.Warning => Diagnostic.DiagnosticSeverity.Warning,
            ReportKind.Error => Diagnostic.DiagnosticSeverity.Error,
            _ => throw new InvalidOperationException("Bad report kind")
        };

        public MappingProfile()
        {
            CreateMap<ReportKind, Diagnostic.DiagnosticSeverity>(MemberList.None)
                .ConvertUsing(x => ToDiagnosticSeverity(x));

            CreateMap<Report, Diagnostic>(MemberList.None)
                .ForMember(x => x.Severity, o => o.MapFrom(x => x.Kind))
                .ForMember(x => x.Summary, o => o.MapFrom(x => x.Header))
                .ForMember(x => x.Detail, o => o.MapFrom(x => x.Body));
        }
    }
}
