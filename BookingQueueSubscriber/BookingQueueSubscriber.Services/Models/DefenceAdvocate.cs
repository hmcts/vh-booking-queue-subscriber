namespace BookingQueueSubscriber.Services.Models;

public class DefenceAdvocate
{
    public string Username { get; }
    public string ContactEmail { get; }

    public DefenceAdvocate(string username, string contactEmail)
    {
        Username = username;
        ContactEmail = contactEmail;
    }
}