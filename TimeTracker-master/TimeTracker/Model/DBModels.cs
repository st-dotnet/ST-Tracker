using System;

namespace DBModels.Model
{
    public class TrackerData
    {
        public Guid TrackerId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan IdleTime { get; set; }
        public Guid? EmployeeId { get; set; }
    }

    public class TrackerScreenshotData
    {
        public int ScreenshotId { get; set; }
        public Guid TrackerId { get; set; }
        public byte[] Screenshots { get; set; }
        public int? Keystrokes { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsCameraCapture { get; set; }
    }

    public class UserInformation
    {
        public string Username { get; set; }
        public Guid? EmployeeId { get; set; }
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
    }
    public class EmployeeData
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }

    }
}
