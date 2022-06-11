﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingQueueSubscriber.Services.MessageHandlers.Dtos;
using BookingQueueSubscriber.Services.UserApi;
using NotificationApi.Client;
using NotificationApi.Contract.Requests;

namespace BookingQueueSubscriber.Services.NotificationApi
{
    public interface INotificationService
    {
        Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password);
        Task SendNewHearingNotification(HearingDto hearing, IList<ParticipantDto> participants);
        Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime,
             IList<ParticipantDto> participants);

        Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationApiClient _notificationApiClient;

        public NotificationService(INotificationApiClient notificationApiClient)
        {
            _notificationApiClient = notificationApiClient;
        }

        public async Task SendNewUserAccountNotificationAsync(Guid hearingId, ParticipantDto participant, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                var request = AddNotificationRequestMapper.MapToNewUserNotification(hearingId, participant, password);
                await _notificationApiClient.CreateNewNotificationAsync(request);
            }
        }
        public async Task SendNewHearingNotification(HearingDto hearing, IList<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants);
                return;
            }

            var requests = participants
                .Select(participant => AddNotificationRequestMapper.MapToNewHearingNotification(hearing, participant))
                .ToList();
            
            await CreateNotifications(requests);
        }

        public async Task SendHearingAmendmentNotificationAsync(HearingDto hearing, DateTime originalDateTime, IList<ParticipantDto> participants)
        {
            if (hearing.IsGenericHearing())
            {
                return;
            }

            var caseName = hearing.CaseName;
            var caseNumber = hearing.CaseNumber;
            //if (!hearing.DoesJudgeEmailExist() || originalDateTime == null ||
            //    originalHearing.GroupId != originalHearing.Id)
            //{
            //    participantsToEmail = participantsToEmail
            //        .Where(x => !x.UserRole.Contains("Judge", StringComparison.CurrentCultureIgnoreCase))
            //        .ToList();
            //}

            var requests = participants
                .Select(participant =>
                    AddNotificationRequestMapper.MapToHearingAmendmentNotification(hearing, participant,
                        originalDateTime, hearing.ScheduledDateTime))
                .ToList();

            await CreateNotifications(requests);
        }

        public async Task SendMultiDayHearingNotificationAsync(HearingDto hearing, IList<ParticipantDto> participants, int days)
        {
            if (hearing.IsGenericHearing())
            {
                await ProcessGenericEmail(hearing, participants); 
                return;
            }

            //if BookAndConfirm toggle switched off include where userRole != Judge LINQ clause to requests
            var requests = participants
                .Select(participant => AddNotificationRequestMapper.MapToMultiDayHearingConfirmationNotification(hearing, participant, days))
                .ToList();

              await CreateNotifications(requests);
        }

        private async Task ProcessGenericEmail(HearingDto hearing, IList<ParticipantDto> participants)
        {
            if (string.Equals(hearing.HearingType, "Automated Test", StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var filteredParticipants = participants
                .Where(x => !x.UserRole.Contains(RoleNames.Judge, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            var notificationRequests = filteredParticipants
                .Select(participant =>
                    AddNotificationRequestMapper.MapToDemoOrTestNotification(hearing, participant, hearing.HearingType))
                .Where(x => x != null)
                .ToList();

            if (!notificationRequests.Any()) return;
            await CreateNotifications(notificationRequests);
        }

        private async Task CreateNotifications(List<AddNotificationRequest> notificationRequests)
        {
            notificationRequests = notificationRequests.Where(req => !string.IsNullOrWhiteSpace(req.ContactEmail)).ToList();
            await Task.WhenAll(notificationRequests.Select(_notificationApiClient.CreateNewNotificationAsync));
        }


    }
}
