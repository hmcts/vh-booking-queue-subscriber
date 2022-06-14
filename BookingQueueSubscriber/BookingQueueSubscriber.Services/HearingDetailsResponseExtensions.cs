using System;
using System.Linq;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
using BookingsApi.Contract.Responses;
using Newtonsoft.Json;

namespace BookingQueueSubscriber.Services
{
    public static class HearingExtensions
    {
        public static bool IsGenericHearing(this HearingDto hearing)
        {
            return hearing.CaseType.Equals("Generic", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool HasScheduleAmended(this HearingDto hearing, HearingDto anotherHearing)
        {
            return hearing.ScheduledDateTime.Ticks != anotherHearing.ScheduledDateTime.Ticks;
        }

        //public static bool JudgeHasNotChangedForGenericHearing(this HearingDto newHearingJudge,
        //    HearingDto originalHearingJudge)
        //{
        //    var judgeFromUpdatedHearing = newHearingJudge.GetJudgeById();
        //    var judgeFromOriginalHearing = originalHearingJudge.GetJudgeById();

        //    if((judgeFromUpdatedHearing != judgeFromOriginalHearing) && newHearingJudge.IsGenericHearing()) return false;
        //    return true;
        //}

        //private static Guid? GetJudgeById(this HearingDto hearing)
        //{
        //    var judgeId = hearing?.Participants.SingleOrDefault(x =>
        //        x.UserRoleName.Contains(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase))?.Id;
        //    return judgeId;
        //}

        //public static bool HasJudgeEmailChanged(this HearingDto hearing,
        //    HearingDto originalHearing)
        //{
        //    var isNewJudgeEJud = IsJudgeEmailEJud(hearing);
        //    var isOriginalJudgeEJud = IsJudgeEmailEJud(originalHearing);
        //    var isNewJudgeVhJudge = hearing.GetJudgeEmail() != null;
        //    var isOriginalJudgeVhJudge = originalHearing.GetJudgeEmail() != null;


        //    if (isNewJudgeEJud && isOriginalJudgeEJud)
        //    {
        //        var judgeA = hearing.Participants.FirstOrDefault(x =>
        //            x.UserRoleName.Contains(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase));


        //        var judgeB = originalHearing.Participants.FirstOrDefault(x =>
        //            x.UserRoleName.Contains(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase));

        //        return judgeA?.ContactEmail != judgeB?.ContactEmail;
        //    }

        //    if (isNewJudgeVhJudge && isOriginalJudgeVhJudge)
        //    {
        //        return hearing.GetJudgeEmail() != originalHearing.GetJudgeEmail();
        //    }

        //    return isNewJudgeEJud || isOriginalJudgeEJud || isNewJudgeVhJudge || isOriginalJudgeVhJudge;
        //}

        //public static bool DoesJudgeEmailExist(this HearingDto hearing, ParticipantDto participant)
        //{
        //    if (participant.IsJudgeEmailEJud())
        //    {
        //        return true;
        //    }

        //    if (hearing.OtherInformation == null) return false;
        //    var otherInformationDetails = GetOtherInformationObject(hearing.OtherInformation);
        //    return !string.IsNullOrEmpty(otherInformationDetails.JudgeEmail);
        //}

        //public static bool DoesJudgePhoneExist(this HearingDto hearing)
        //{
        //    if (hearing.OtherInformation == null) return false;
        //    var otherInformationDetails = GetOtherInformationObject(hearing.OtherInformation);
        //    return !string.IsNullOrWhiteSpace(otherInformationDetails.JudgePhone);
        //}

        //public static string GetJudgeEmail(this HearingDto hearing)
        //{

        //    var email = GetOtherInformationObject(hearing.OtherInformation)?.JudgeEmail;
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return null;
        //    }

        //    return email;
        //}

        public static bool IsJudgeEmailEJud(this ParticipantDto participant)
        {
            return participant.UserRole.Equals(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase) &&
                   IsEmailEjud(participant);
        }

        public static bool IsParticipantAEJudJudicialOfficeHolder(this ParticipantDto participant)
        {
            return participant.UserRole.Equals(RoleNames.JudicialOfficeHolder, StringComparison.CurrentCultureIgnoreCase) &&
                   IsEmailEjud(participant);
        }

        public static bool IsParticipantAJudicialOfficeHolderOrJudge(this ParticipantDto participant)
        {
            var joh = participant.UserRole.Equals(RoleNames.JudicialOfficeHolder, StringComparison.CurrentCultureIgnoreCase);
            var judge = participant.UserRole.Equals(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase);
            return joh || judge;
        }

        private static bool IsEmailEjud(this ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.Username) && participant.Username.Contains("judiciary", StringComparison.CurrentCultureIgnoreCase);
        }

        //public static string GetJudgePhone(this HearingDto hearing)
        //{
        //    var phone = GetOtherInformationObject(hearing.OtherInformation).JudgePhone;
        //    if (phone == string.Empty)
        //    {
        //        return null;
        //    }

        //    return phone;
        //}

        public static string ToOtherInformationString(this OtherInformationDetails otherInformationDetailsObject)
        {
            return
                $"|JudgeEmail|{otherInformationDetailsObject.JudgeEmail}" +
                $"|JudgePhone|{otherInformationDetailsObject.JudgePhone}" +
                $"|OtherInformation|{otherInformationDetailsObject.OtherInformation}";
        }

        public static HearingDetailsResponse Duplicate(this HearingDto hearingDetailsResponse)
        {
            var json = JsonConvert.SerializeObject(hearingDetailsResponse);
            return JsonConvert.DeserializeObject<HearingDetailsResponse>(json);
        }

        private static OtherInformationDetails GetOtherInformationObject(string otherInformation)
        {
            try
            {
                var properties = otherInformation.Split("|");
                var otherInfo = new OtherInformationDetails
                {
                    JudgeEmail = Array.IndexOf(properties, "JudgeEmail") > -1
                        ? properties[Array.IndexOf(properties, "JudgeEmail") + 1]
                        : "",
                    JudgePhone = Array.IndexOf(properties, "JudgePhone") > -1
                        ? properties[Array.IndexOf(properties, "JudgePhone") + 1]
                        : "",
                    OtherInformation = Array.IndexOf(properties, "OtherInformation") > -1
                        ? properties[Array.IndexOf(properties, "OtherInformation") + 1]
                        : ""
                };
                return otherInfo;
            }
            catch (Exception)
            {
                if (string.IsNullOrWhiteSpace(otherInformation))
                {
                    return new OtherInformationDetails { OtherInformation = otherInformation };
                }

                var properties = otherInformation.Split("|");
                if (properties.Length > 2)
                {
                    return new OtherInformationDetails { OtherInformation = properties[2] };
                }

                return new OtherInformationDetails { OtherInformation = otherInformation };
            }
        }
    }
}