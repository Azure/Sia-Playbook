using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Domain.Playbook;
using Sia.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Initialization
{
    public static class AutoMapperStartup
    {
        public static void InitializeAutomapper()
            => Mapper.Initialize(configuration =>
            {
                configuration.AddCollectionMappers();

                configuration.CreateMap<CreateEventType, Data.Playbooks.Models.EventType>()
                    .EqualityInsertOnly()
                    .UseResolveJsonToString();
                configuration.CreateMap<EventType, Data.Playbooks.Models.EventType>()
                    .EqualityById()
                    .UseResolveJsonToString();
                configuration.CreateMap<Data.Playbooks.Models.EventType, EventType>()
                    .EqualityById()
                    .UseResolveStringToJson();

                configuration.CreateMap<CreateAction, Data.Playbooks.Models.Action>()
                    .EqualityInsertOnly();
                configuration.CreateMap<Domain.Playbook.Action, Data.Playbooks.Models.Action>()
                    .EqualityById();
                configuration.CreateMap<Data.Playbooks.Models.Action, Domain.Playbook.Action>()
                    .EqualityById();

                configuration.CreateMap<CreateActionTemplate, Data.Playbooks.Models.ActionTemplate>()
                    .EqualityInsertOnly();
                configuration.CreateMap<ActionTemplate, Data.Playbooks.Models.ActionTemplate>()
                    .EqualityById();
                configuration.CreateMap<Data.Playbooks.Models.ActionTemplate, ActionTemplate>()
                    .EqualityById();

                configuration.CreateMap<CreateActionTemplateSource, Data.Playbooks.Models.ActionTemplateSource>()
                    .EqualityInsertOnly();
                configuration.CreateMap<ActionTemplateSource, Data.Playbooks.Models.ActionTemplateSource>()
                    .EqualityById();
                configuration.CreateMap<Data.Playbooks.Models.ActionTemplateSource, ActionTemplateSource>()
                    .EqualityById();

                configuration.CreateMap<CreateCondition, Data.Playbooks.Models.Condition>()
                    .EqualityInsertOnly();
                configuration.CreateMap<Condition, Data.Playbooks.Models.Condition>()
                    .EqualityById();
                configuration.CreateMap<Data.Playbooks.Models.Condition, Condition>()
                    .EqualityById();

                configuration.CreateMap<CreateConditionSet, Data.Playbooks.Models.ConditionSet>()
                    .EqualityInsertOnly();
                configuration.CreateMap<ConditionSet, Data.Playbooks.Models.ConditionSet>()
                    .EqualityById();
                configuration.CreateMap<Data.Playbooks.Models.ConditionSet, ConditionSet>()
                    .EqualityById();

                configuration.CreateMap<CreateConditionSource, Data.Playbooks.Models.ConditionSource>()
                    .EqualityInsertOnly();
                configuration.CreateMap<ConditionSource, Data.Playbooks.Models.ConditionSource>()
                    .EqualityById();
                configuration.CreateMap<Data.Playbooks.Models.ConditionSource, ConditionSource>()
                    .EqualityById();
            });

        private static IMappingExpression<TSource, TDestination> UseResolveJsonToString<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : IJsonDataObject
            where TDestination : IJsonDataString
            => mapping.ForMember((ev) => ev.Data, (config) => config.ResolveUsing<ResolveJsonToString<TSource, TDestination>>());


        private static IMappingExpression<TSource, TDestination> UseResolveStringToJson<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : IJsonDataString
            where TDestination : IJsonDataObject
            => mapping.ForMember((ev) => ev.Data, (config) => config.ResolveUsing<ResolveStringToJson<TSource, TDestination>>());


        public static IMappingExpression<T1, T2> EqualityInsertOnly<T1, T2>(this IMappingExpression<T1, T2> mappingExpression)
            where T1 : class
            where T2 : class
            => mappingExpression.EqualityComparison((one, two) => false);

        public static IMappingExpression<T1, T2> EqualityById<T1, T2>(this IMappingExpression<T1, T2> mappingExpression)
            where T1 : class, IEntity
            where T2 : class, IEntity
            => mappingExpression.EqualityComparison((one, two) => one.Id == two.Id);
    }
}

