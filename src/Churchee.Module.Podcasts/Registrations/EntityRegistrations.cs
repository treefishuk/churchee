﻿using Churchee.Common.Abstractions.Storage;
using Churchee.Module.Podcasts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Podcasts.Registrations
{
    public class EntityRegistrations : IEntityRegistration
    {
        public void RegisterEntities(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Podcast>(etb =>
            {
                etb.ToTable("Podcasts");
                etb.Property(e => e.Id);
            });

        }
    }
}
