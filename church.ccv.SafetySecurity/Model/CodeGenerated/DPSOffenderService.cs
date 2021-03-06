//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
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
using System;
using System.Linq;

using Rock.Data;

namespace church.ccv.SafetySecurity.Model
{
    /// <summary>
    /// DPSOffender Service class
    /// </summary>
    public partial class DPSOffenderService : Service<DPSOffender>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DPSOffenderService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public DPSOffenderService(RockContext context) : base(context)
        {
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( DPSOffender item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class DPSOffenderExtensionMethods
    {
        /// <summary>
        /// Clones this DPSOffender object to a new DPSOffender object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static DPSOffender Clone( this DPSOffender source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as DPSOffender;
            }
            else
            {
                var target = new DPSOffender();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another DPSOffender object to this DPSOffender object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this DPSOffender target, DPSOffender source )
        {
            target.Id = source.Id;
            target.Absconder = source.Absconder;
            target.Age = source.Age;
            target.ConfirmedMatch = source.ConfirmedMatch;
            target.ConvictionState = source.ConvictionState;
            target.DateConvicted = source.DateConvicted;
            target.DpsLocationId = source.DpsLocationId;
            target.Eyes = source.Eyes;
            target.FirstName = source.FirstName;
            target.ForeignGuid = source.ForeignGuid;
            target.ForeignKey = source.ForeignKey;
            target.Gender = source.Gender;
            target.Hair = source.Hair;
            target.Height = source.Height;
            target.LastName = source.LastName;
            target.Level = source.Level;
            target.MiddleInitial = source.MiddleInitial;
            target.Offense = source.Offense;
            target.PersonAliasId = source.PersonAliasId;
            target.Race = source.Race;
            target.ResAddress = source.ResAddress;
            target.ResCity = source.ResCity;
            target.ResState = source.ResState;
            target.ResZip = source.ResZip;
            target.Weight = source.Weight;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
