﻿using System.Collections.Generic;
using System.Linq;
using Boerman.GraphQL.Contrib.DataLoaders;
using GraphQL.DataLoader;
using GraphQL.Types;
using SchoolEfCore.Entities;
using SchoolEfCore.Interfaces;

namespace BasicCollectionLoaderTest.Types
{
    public sealed class ClassType : ObjectGraphType<Class>
    {
        public ClassType(IDataLoaderContextAccessor accessor, ISchoolContext schoolContext)
        {
            Field(_ => _.Id, type: typeof(IdGraphType))
                .Description("The class' ID.");

            Field(_ => _.Subject)
                .Description("The class' subject.");

            Field(_ => _.Pupils, type: typeof(ListGraphType<PupilType>))
                .Description("The class' pupils.")
                .ResolveAsync(
                    async context =>
                    {
                        IList<Pupil> list = (await accessor.EntityCollectionLoader(
                                () => schoolContext.ClassPupil,
                                classesPupils => classesPupils.ClassId,
                                select => select.Pupil,
                                pupil => pupil.Id,
                                context.Source.Id)
                            .ConfigureAwait(false)).ToList();
                        return list;
                    });
        }
    }
}
