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
    /// DatamartERA Service class
    /// </summary>
    public partial class DatamartERAService : Service<DatamartERA>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatamartERAService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public DatamartERAService(church.ccv.Datamart.Data.DatamartContext context) : base(context)
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
        public bool CanDelete( DatamartERA item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class DatamartERAExtensionMethods
    {
        /// <summary>
        /// Clones this DatamartERA object to a new DatamartERA object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static DatamartERA Clone( this DatamartERA source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as DatamartERA;
            }
            else
            {
                var target = new DatamartERA();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another DatamartERA object to this DatamartERA object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this DatamartERA target, DatamartERA source )
        {
            target.Id = source.Id;
            target.FamilyId = source.FamilyId;
            target.FirstAttended = source.FirstAttended;
            target.LastAttended = source.LastAttended;
            target.LastGave = source.LastGave;
            target.RegularAttendee = source.RegularAttendee;
            target.RegularAttendeeC = source.RegularAttendeeC;
            target.RegularAttendeeG = source.RegularAttendeeG;
            target.TimesAttendedLast16Weeks = source.TimesAttendedLast16Weeks;
            target.TimesGaveLast6Weeks = source.TimesGaveLast6Weeks;
            target.TimesGaveLastYear = source.TimesGaveLastYear;
            target.TimesGaveTotal = source.TimesGaveTotal;
            target.WeekendDate = source.WeekendDate;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
