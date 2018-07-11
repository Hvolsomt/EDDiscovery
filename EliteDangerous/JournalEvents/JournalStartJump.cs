﻿/*
 * Copyright © 2017 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 *
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */
using Newtonsoft.Json.Linq;
using System.Linq;

namespace EliteDangerousCore.JournalEvents
{
    [JournalEntryType(JournalTypeEnum.StartJump)]
    public class JournalStartJump : JournalEntry
    {
        public JournalStartJump(JObject evt) : base(evt, JournalTypeEnum.StartJump)
        {
            JumpType = evt["JumpType"].Str();
            IsHyperspace = JumpType.Equals("Hyperspace", System.StringComparison.InvariantCultureIgnoreCase);
            StarSystem = evt["StarSystem"].Str();
            StarClass = evt["StarClass"].Str();
            FriendlyStarClass = (StarClass.Length > 0) ? Bodies.StarName(Bodies.StarStr2Enum(StarClass)) : "";
            SystemAddress = evt["SystemAddress"].LongNull();
        }

        public string JumpType { get; set; }            // Hyperspace, Supercruise
        public bool IsHyperspace { get; set; }
        public string StarSystem { get; set; }
        public long? SystemAddress { get; set; }
        public string StarClass { get; set; }
        public string FriendlyStarClass { get; set; }

        public override string FillSummary { get { return "Charging FSD".Tx(this); } }

        public override void FillInformation(out string info, out string detailed) 
        {
            info = BaseUtils.FieldBuilder.Build("", JumpType, "< to ".Txb(this), StarSystem, "" , FriendlyStarClass);
            detailed = "";
        }
    }
}
