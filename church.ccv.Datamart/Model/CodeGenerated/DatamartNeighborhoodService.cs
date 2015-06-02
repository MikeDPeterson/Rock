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

namespace church.ccv.Datamart.Model
{
    /// <summary>
    /// DatamartNeighborhood Service class
    /// </summary>
    public partial class DatamartNeighborhoodService : Service<DatamartNeighborhood>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatamartNeighborhoodService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public DatamartNeighborhoodService(church.ccv.Datamart.Data.DatamartContext context) : base(context)
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
        public bool CanDelete( DatamartNeighborhood item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class DatamartNeighborhoodExtensionMethods
    {
        /// <summary>
        /// Clones this DatamartNeighborhood object to a new DatamartNeighborhood object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static DatamartNeighborhood Clone( this DatamartNeighborhood source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as DatamartNeighborhood;
            }
            else
            {
                var target = new DatamartNeighborhood();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another DatamartNeighborhood object to this DatamartNeighborhood object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this DatamartNeighborhood target, DatamartNeighborhood source )
        {
            target.Id = source.Id;
            target.AdultAttendeeCount = source.AdultAttendeeCount;
            target.AdultAttendeesInGroups = source.AdultAttendeesInGroups;
            target.AdultAttendeesInGroupsPercentage = source.AdultAttendeesInGroupsPercentage;
            target.AdultCount = source.AdultCount;
            target.AdultMemberCount = source.AdultMemberCount;
            target.AdultMembersInGroups = source.AdultMembersInGroups;
            target.AdultMembersInGroupsPercentage = source.AdultMembersInGroupsPercentage;
            target.AdultParticipants = source.AdultParticipants;
            target.AdultsBaptized = source.AdultsBaptized;
            target.AdultsBaptizedPercentage = source.AdultsBaptizedPercentage;
            target.AdultsInGroups = source.AdultsInGroups;
            target.AdultsInGroupsPercentage = source.AdultsInGroupsPercentage;
            target.AdultVisitors = source.AdultVisitors;
            target.ChildrenCount = source.ChildrenCount;
            target.GroupCount = source.GroupCount;
            target.HouseholdCount = source.HouseholdCount;
            target.NeighborhoodId = source.NeighborhoodId;
            target.NeighborhoodLeaderId = source.NeighborhoodLeaderId;
            target.NeighborhoodLeaderName = source.NeighborhoodLeaderName;
            target.NeighborhoodName = source.NeighborhoodName;
            target.NeighborhoodPastorId = source.NeighborhoodPastorId;
            target.NeighborhoodPastorName = source.NeighborhoodPastorName;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
