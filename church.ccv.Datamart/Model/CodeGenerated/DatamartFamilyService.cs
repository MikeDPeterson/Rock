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
    /// DatamartFamily Service class
    /// </summary>
    public partial class DatamartFamilyService : Service<DatamartFamily>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatamartFamilyService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public DatamartFamilyService(church.ccv.Datamart.Data.DatamartContext context) : base(context)
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
        public bool CanDelete( DatamartFamily item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class DatamartFamilyExtensionMethods
    {
        /// <summary>
        /// Clones this DatamartFamily object to a new DatamartFamily object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static DatamartFamily Clone( this DatamartFamily source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as DatamartFamily;
            }
            else
            {
                var target = new DatamartFamily();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another DatamartFamily object to this DatamartFamily object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this DatamartFamily target, DatamartFamily source )
        {
            target.Id = source.Id;
            target.Address = source.Address;
            target.AdultCount = source.AdultCount;
            target.AdultNames = source.AdultNames;
            target.Attendance16Week = source.Attendance16Week;
            target.Campus = source.Campus;
            target.ChildCount = source.ChildCount;
            target.ChildNames = source.ChildNames;
            target.City = source.City;
            target.ConnectionStatus = source.ConnectionStatus;
            target.Contrib2007 = source.Contrib2007;
            target.Contrib2008 = source.Contrib2008;
            target.Contrib2009 = source.Contrib2009;
            target.Contrib2010 = source.Contrib2010;
            target.Contrib2011 = source.Contrib2011;
            target.Contrib2012 = source.Contrib2012;
            target.Contrib2013 = source.Contrib2013;
            target.Contrib2014 = source.Contrib2014;
            target.Contrib2015 = source.Contrib2015;
            target.Country = source.Country;
            target.Email = source.Email;
            target.FamilyName = source.FamilyName;
            target.GeoPoint = source.GeoPoint;
            target.HHAge = source.HHAge;
            target.HHFirstActivity = source.HHFirstActivity;
            target.HHFirstName = source.HHFirstName;
            target.HHFirstVisit = source.HHFirstVisit;
            target.HHFullName = source.HHFullName;
            target.HHGender = source.HHGender;
            target.HHLastName = source.HHLastName;
            target.HHMaritalStatus = source.HHMaritalStatus;
            target.HHMemberStatus = source.HHMemberStatus;
            target.HHNickName = source.HHNickName;
            target.HHPersonId = source.HHPersonId;
            target.HomePhone = source.HomePhone;
            target.InNeighborhoodGroup = source.InNeighborhoodGroup;
            target.IsEra = source.IsEra;
            target.IsServing = source.IsServing;
            target.Latitude = source.Latitude;
            target.LocationId = source.LocationId;
            target.Longitude = source.Longitude;
            target.NearestNeighborhoodGroupId = source.NearestNeighborhoodGroupId;
            target.NearestNeighborhoodGroupName = source.NearestNeighborhoodGroupName;
            target.NeighborhoodId = source.NeighborhoodId;
            target.NeighborhoodName = source.NeighborhoodName;
            target.PostalCode = source.PostalCode;
            target.State = source.State;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
