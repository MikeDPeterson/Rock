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
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using System.Data.Entity;

namespace church.ccv.MobileApp.Models
{
    [Serializable]
    public class GroupRegModel
    {
        public int RequestedGroupId;

        public string FirstName;
        public string LastName;
        public string SpouseName;
        public string Email;
        public string Phone;
    }

    [Serializable]
    public class GroupInfo
    {
        public string Description { get; set; }
        public string ChildcareDesc { get; set; }
        
        public Guid FamilyPhotoGuid { get; set; }

        public string Filters { get; set; }
    }

    [Serializable]
    public class GroupResult
    {
        public int Id;

        public double Latitude;
        public double Longitude;

        public string Name;
        public double DistanceFromSource;
         
        public string MeetingTime;

        public string Filters;
    }

    [Serializable]
    public class PersonData
    {
        public Person Person;
        
        public PhoneNumber CellPhoneNumber;

        public Group Family;
        public GroupLocation FamilyAddress;

        // Actions
        public bool IsBaptised;

        public bool IsERA;

        public bool IsGiving;

        public bool TakenStartingPoint;

        public bool IsMember;

        public bool IsServing;

        public bool IsPeerLearning;

        public bool IsMentored;

        public bool IsTeaching;

        public bool SharedStory;
    }

    [Serializable]
    public class LaunchData
    {
        public int MobileAppVersion;

        // all the mobile app cares about are the strings and IDs, so that's all we'll return
        public List<KeyValuePair<string, int>> PrayerCategories;

        // campuses need the guid, name and ID, so we'll pass down the entire model
        public List<Campus> Campuses;
    }

    [Serializable]
    public class RegAccountData
    {
        public string FirstName;
        public string LastName;
        public string Email;

        public string CellPhoneNumber;

        public string Username;
        public string Password;
    }
}
