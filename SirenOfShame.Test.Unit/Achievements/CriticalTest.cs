﻿using System.Collections.Generic;
using NUnit.Framework;
using SirenOfShame.Lib.Achievements;
using SirenOfShame.Lib.Settings;
using SirenOfShame.Lib.StatCalculators;
using SirenOfShame.Lib.Watcher;

namespace SirenOfShame.Test.Unit.Achievements
{
    [TestFixture]
    public class CriticalTest
    {
        [Test]
        public void CurrentBuildRatio()
        {
            PersonSetting personSetting = new PersonSetting { RawName = "currentUser" };
            List<BuildStatus> builds = new List<BuildStatus>();
            builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Broken });
            builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Working });
            builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Working });
            builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Working });
            builds.Add(new BuildStatus { RequestedBy = "someoneElse", CurrentBuildStatus = BuildStatusEnum.Working });
            Assert.AreEqual(0.25, BuildRatio.CalculateCurrentBuildRatio(personSetting, builds));
        }

        [Test]
        public void Exactly50BuildsOneFailedLowestPercentageIsTwoPercent()
        {
            PersonSetting personSetting = new PersonSetting { RawName = "currentUser"};
            List<BuildStatus> builds = new List<BuildStatus>();
            builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Broken });
            for (int i = 0; i < 49; i++)
            {
                builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Working });
            }
            Assert.AreEqual(50, builds.Count);
            Assert.AreEqual(0.02, BuildRatio.CalculateLowestBuildRatioAfter50Builds(personSetting, builds));
        }
        
        [Test]
        public void Exactly100BuildsOneFailedLowestPercentageIsOnePercent()
        {
            PersonSetting personSetting = new PersonSetting { RawName = "currentUser"};
            List<BuildStatus> builds = new List<BuildStatus>();
            builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Broken });
            for (int i = 0; i < 99; i++)
            {
                builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Working });
            }
            Assert.AreEqual(100, builds.Count);
            Assert.AreEqual(0.01, BuildRatio.CalculateLowestBuildRatioAfter50Builds(personSetting, builds));
        }
        
        [Test]
        public void AchievesTwoPercentAt50ThenFails50More_PercentageStaysAtTwo()
        {
            PersonSetting personSetting = new PersonSetting { RawName = "currentUser"};
            List<BuildStatus> builds = new List<BuildStatus>();
            builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Broken });
            for (int i = 0; i < 49; i++)
            {
                builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Working });
            }
            for (int i = 0; i < 50; i++)
            {
                builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Broken });
            }
            Assert.AreEqual(100, builds.Count);
            Assert.AreEqual(0.02, BuildRatio.CalculateLowestBuildRatioAfter50Builds(personSetting, builds));
        }
        
        [Test]
        public void Exactly50BuildsOneFailed_ButFailedBuildIsSomeoneElse()
        {
            PersonSetting personSetting = new PersonSetting { RawName = "currentUser"};
            List<BuildStatus> builds = new List<BuildStatus>();
            builds.Add(new BuildStatus { RequestedBy = "someoneElse", CurrentBuildStatus = BuildStatusEnum.Broken });
            for (int i = 0; i < 49; i++)
            {
                builds.Add(new BuildStatus { RequestedBy = "currentUser", CurrentBuildStatus = BuildStatusEnum.Working });
            }
            Assert.AreEqual(50, builds.Count);
            Assert.IsNull(BuildRatio.CalculateLowestBuildRatioAfter50Builds(personSetting, builds));
        }
    }
}
