using FluentAssertions;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain;
using HousingRegisterApi.V1.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HousingRegisterApi.Tests.V1.Infrastructure
{
    public class ResidentUpdateRestrictionsTests
    {
        private static readonly Guid _mainId = Guid.NewGuid();
        private static readonly Guid _memberAId = Guid.NewGuid();
        private static readonly Guid _memberBId = Guid.NewGuid();

        private static MedicalNeed StaffMedicalNeed(string outcome = "approved") =>
            new MedicalNeed { FormRecieved = new DateTime(2026, 1, 2), Outcome = outcome };

        private static Applicant Resident(Guid id, string firstName, MedicalNeed medicalNeed = null) =>
            new Applicant
            {
                Person = new Person { Id = id, FirstName = firstName },
                Address = new Address { Postcode = "E8 1AA" },
                ContactInformation = new ContactInformation { EmailAddress = $"{firstName}@example.test", PhoneNumber = "0123" },
                Questions = new List<Question> { new Question { Id = "q1", Answer = $"{firstName}-answer" } },
                MedicalNeed = medicalNeed,
            };

        private static Application Stored(string status, params Applicant[] otherMembers) =>
            new Application
            {
                Status = status,
                MainApplicant = Resident(_mainId, "Stored", StaffMedicalNeed()),
                OtherMembers = otherMembers.ToList(),
            };

        [Test]
        public void StaffOwnedTopLevelFieldsAreRemoved()
        {
            var request = new UpdateApplicationRequest
            {
                SensitiveData = true,
                AssignedTo = "someone@hackney.gov.uk",
                Assessment = new Assessment { Band = "A" },
                Status = ApplicationStatus.Cancelled,
            };

            ResidentUpdateRestrictions.Apply(request, Stored(ApplicationStatus.Submitted));

            request.SensitiveData.Should().BeNull();
            request.AssignedTo.Should().BeNull();
            request.Assessment.Should().BeNull();
            request.Status.Should().BeNull();
        }

        [Test]
        public void NullRequestIsIgnored()
        {
            Action act = () => ResidentUpdateRestrictions.Apply(null, Stored(ApplicationStatus.New));

            act.Should().NotThrow();
        }

        [Test]
        public void HouseholdIsLeftNullWhenTheRequestDoesNotSendIt()
        {
            var request = new UpdateApplicationRequest { Status = ApplicationStatus.Cancelled };

            ResidentUpdateRestrictions.Apply(request, Stored(ApplicationStatus.Submitted, Resident(_memberAId, "A")));

            request.MainApplicant.Should().BeNull();
            request.OtherMembers.Should().BeNull();
        }

        [Test]
        public void MainApplicantTakesResidentDetailsAndKeepsStoredMedicalNeed()
        {
            var requested = Resident(_mainId, "Edited", new MedicalNeed { Outcome = "injected" });
            var request = new UpdateApplicationRequest { MainApplicant = requested };

            ResidentUpdateRestrictions.Apply(request, Stored(ApplicationStatus.Submitted));

            request.MainApplicant.Person.FirstName.Should().Be("Edited");
            request.MainApplicant.Address.Should().BeSameAs(requested.Address);
            request.MainApplicant.ContactInformation.Should().BeSameAs(requested.ContactInformation);
            request.MainApplicant.Questions.Should().BeSameAs(requested.Questions);
            request.MainApplicant.MedicalNeed.Should().BeEquivalentTo(StaffMedicalNeed());
        }

        [Test]
        public void MainApplicantMedicalNeedStaysEmptyWhenNoneIsStored()
        {
            var stored = Stored(ApplicationStatus.Submitted);
            stored.MainApplicant.MedicalNeed = null;
            var request = new UpdateApplicationRequest
            {
                MainApplicant = Resident(_mainId, "Edited", new MedicalNeed { Outcome = "injected" }),
            };

            ResidentUpdateRestrictions.Apply(request, stored);

            request.MainApplicant.MedicalNeed.Should().BeNull();
        }

        [Test]
        public void MissingMainApplicantPartsFallBackToTheStoredOnes()
        {
            var stored = Stored(ApplicationStatus.Submitted);
            var request = new UpdateApplicationRequest
            {
                MainApplicant = new Applicant { Person = new Person { Id = _mainId, FirstName = "Edited" } },
            };

            ResidentUpdateRestrictions.Apply(request, stored);

            request.MainApplicant.Person.FirstName.Should().Be("Edited");
            request.MainApplicant.Address.Should().BeSameAs(stored.MainApplicant.Address);
            request.MainApplicant.ContactInformation.Should().BeSameAs(stored.MainApplicant.ContactInformation);
            request.MainApplicant.Questions.Should().BeSameAs(stored.MainApplicant.Questions);
        }

        [Test]
        public void MainApplicantOfAnApplicationWithNoneStoredHasNoMedicalNeed()
        {
            var request = new UpdateApplicationRequest
            {
                MainApplicant = Resident(_mainId, "New", new MedicalNeed { Outcome = "injected" }),
            };

            ResidentUpdateRestrictions.Apply(request, new Application { Status = ApplicationStatus.New });

            request.MainApplicant.Person.FirstName.Should().Be("New");
            request.MainApplicant.MedicalNeed.Should().BeNull();
        }

        [TestCase(ApplicationStatus.New)]
        [TestCase(ApplicationStatus.ManualDraft)]
        public void DraftHouseholdFollowsTheResidentsListAndKeepsStaffMedicalNeeds(string status)
        {
            var stored = Stored(
                status,
                Resident(_memberAId, "A", StaffMedicalNeed("a")),
                Resident(_memberBId, "B", StaffMedicalNeed("b")));
            var newMemberId = Guid.NewGuid();
            var request = new UpdateApplicationRequest
            {
                OtherMembers = new List<Applicant>
                {
                    Resident(_memberAId, "A-edited"),
                    Resident(newMemberId, "New", new MedicalNeed { Outcome = "injected" }),
                },
            };

            ResidentUpdateRestrictions.Apply(request, stored);

            var members = request.OtherMembers.ToList();
            members.Select(m => m.Person.Id).Should().Equal(_memberAId, newMemberId);
            members[0].Person.FirstName.Should().Be("A-edited");
            members[0].MedicalNeed.Should().BeEquivalentTo(StaffMedicalNeed("a"));
            members[1].MedicalNeed.Should().BeNull();
        }

        [TestCase(ApplicationStatus.Submitted)]
        [TestCase(ApplicationStatus.Cancelled)]
        [TestCase(ApplicationStatus.Active)]
        [TestCase(ApplicationStatus.Pending)]
        public void HouseholdMembershipIsStaffControlledOncePastDraft(string status)
        {
            var stored = Stored(
                status,
                Resident(_memberAId, "A", StaffMedicalNeed("a")),
                Resident(_memberBId, "B", StaffMedicalNeed("b")));
            var request = new UpdateApplicationRequest
            {
                // Stale copy: knows nothing about member B and invents a new member
                OtherMembers = new List<Applicant>
                {
                    Resident(_memberAId, "A-edited"),
                    Resident(Guid.NewGuid(), "Invented"),
                },
            };

            ResidentUpdateRestrictions.Apply(request, stored);

            var members = request.OtherMembers.ToList();
            members.Select(m => m.Person.Id).Should().Equal(_memberAId, _memberBId);
            members[0].Person.FirstName.Should().Be("A-edited");
            members[0].MedicalNeed.Should().BeEquivalentTo(StaffMedicalNeed("a"));
            members[1].Person.FirstName.Should().Be("B");
            members[1].MedicalNeed.Should().BeEquivalentTo(StaffMedicalNeed("b"));
        }

        [Test]
        public void EmptyHouseholdFromAStaleCopyDoesNotWipeMembersOncePastDraft()
        {
            var stored = Stored(ApplicationStatus.Submitted, Resident(_memberAId, "A"));
            var request = new UpdateApplicationRequest { OtherMembers = new List<Applicant>() };

            ResidentUpdateRestrictions.Apply(request, stored);

            request.OtherMembers.Select(m => m.Person.Id).Should().Equal(_memberAId);
        }

        [Test]
        public void MembersWithoutAMatchableIdAreNotMergedOntoStoredOnes()
        {
            var stored = Stored(ApplicationStatus.New, Resident(Guid.Empty, "Stored", StaffMedicalNeed()));
            var request = new UpdateApplicationRequest
            {
                OtherMembers = new List<Applicant> { Resident(Guid.Empty, "Incoming") },
            };

            ResidentUpdateRestrictions.Apply(request, stored);

            var member = request.OtherMembers.Single();
            member.Person.FirstName.Should().Be("Incoming");
            member.MedicalNeed.Should().BeNull();
        }

        [Test]
        public void DisqualifyingADraftKeepsTheStatus()
        {
            var request = new UpdateApplicationRequest { Status = ApplicationStatus.Disqualified };

            ResidentUpdateRestrictions.Apply(request, Stored(ApplicationStatus.New));

            request.Status.Should().Be(ApplicationStatus.Disqualified);
        }

        [Test]
        public void DisqualifyingAnApplicationPastDraftIsIgnored()
        {
            var request = new UpdateApplicationRequest { Status = ApplicationStatus.Disqualified };

            ResidentUpdateRestrictions.Apply(request, Stored(ApplicationStatus.Submitted));

            request.Status.Should().BeNull();
        }
    }
}
