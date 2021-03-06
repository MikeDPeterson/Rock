﻿// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using church.ccv.Actions.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace church.ccv.Actions.Data
{
    public partial class ActionsContext : Rock.Data.DbContext
    {
        public ActionsContext()
            : base( "RockContext" )
        {
            // intentionally left blank
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            // we don't want this context to create a database or look for EF Migrations, do set the Initializer to null
            Database.SetInitializer<ActionsContext>( new NullDatabaseInitializer<ActionsContext>() );

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly( typeof( ActionsContext ).Assembly );
        }
        
        #region Models
        DbSet<ActionsHistory_Adult> ActionsHistory_Adult { get; set; }
        DbSet<ActionsHistory_Student> ActionsHistory_Student { get; set; }

        DbSet<ActionsHistory_Adult_Person> ActionsHistory_Adult_Person { get; set; }
        DbSet<ActionsHistory_Student_Person> ActionsHistory_Student_Person { get; set; }
        #endregion
    }
}
