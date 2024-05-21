﻿using DBModels.Model;
using Microsoft.Identity.Client;
using System;
using System.IO;
using System.Xml.Linq;

namespace TimeTracker.Utilities
{
    public class UserInformationManager
    {
        private readonly string _filePath;

        public UserInformationManager(string filePath)
        {
            _filePath = filePath;
        }
        #region User Manager XML
        public void SaveUserInformation(AuthenticationResult result, Guid? empId)
        {
            if (File.Exists(_filePath))
            {
                XDocument xmlDocument1 = XDocument.Load(_filePath);

                XElement userElement = xmlDocument1.Element("User");

                if (userElement != null)
                {
                    File.Delete(_filePath);
                }
            }
                    XDocument xmlDocument = new XDocument(
                    new XElement("User",
                    new XElement("Username", result.Account.Username),
                    new XElement("EmployeeId", empId),
                    new XElement("IdToken", result.IdToken),
                    new XElement("AccessToken", result.AccessToken),
                    new XElement("ExpiresOn", result.ExpiresOn)
                )
            );

            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                xmlDocument.Save(writer);
            }
        }

        public void RemoveUserInformation()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        public UserInformation RetrieveUserInformation()
        {
            if (File.Exists(_filePath))
            {
                XDocument xmlDocument = XDocument.Load(_filePath);

                XElement userElement = xmlDocument.Element("User");

                if (userElement != null)
                {
                    return new UserInformation
                    {
                        Username = userElement.Element("Username")?.Value,
                        EmployeeId = Guid.Parse(userElement.Element("EmployeeId")?.Value),
                        IdToken = userElement.Element("IdToken")?.Value,
                        AccessToken = userElement.Element("AccessToken")?.Value,
                        ExpiresOn = DateTimeOffset.Parse(userElement.Element("ExpiresOn")?.Value)
                    };
                }
            }

            return null;
        }
        #endregion

        #region Local Tracker Data XML
        public void SaveTrackerDataOffline(TrackerDataOffline result)
        {
            if (File.Exists(_filePath))
            {
                XDocument xmlDocument = XDocument.Load(_filePath);

                XElement trackerDataElement = xmlDocument.Element("TrackerData");

                if (trackerDataElement != null)
                {
                    var oldTotalTimeString = trackerDataElement.Element("TotalTime")?.Value;
                    TimeSpan.TryParse(oldTotalTimeString, out TimeSpan oldTotalTime);
                    var oldIdleTimeString = trackerDataElement.Element("IdleTime")?.Value;
                    TimeSpan.TryParse(oldIdleTimeString, out TimeSpan oldIdleTime);
                    File.Delete(_filePath);
                    TimeSpan newTotalTime = oldTotalTime + result.TotalTime;
                    result.TotalTime = newTotalTime;
                    TimeSpan newIdleTime = oldIdleTime + result.IdleTime;
                    result.IdleTime = newIdleTime;
                }
            }
            XDocument xmlDocument1 = new XDocument(
                new XElement("TrackerData",
                    new XElement("TrackerId", result.TrackerId),
                    new XElement("Date", result.Date),
                    new XElement("TotalTime", string.Format("{0:hh\\:mm\\:ss\\.fffffff}", result.TotalTime)),
                    new XElement("IdleTime", string.Format("{0:hh\\:mm\\:ss\\.fffffff}", result.IdleTime)),
                    new XElement("EmployeeUsername", result.EmployeeUsername)
                    )
                );

            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                xmlDocument1.Save(writer);
            }
        }
        public TrackerDataOffline RetrieveTrackerDataIfExists()
        {
            if (File.Exists(_filePath))
            {
                XDocument xmlDocument = XDocument.Load(_filePath);

                XElement trackerDataElement = xmlDocument.Element("TrackerData");
                if (trackerDataElement != null)
                {
                    return new TrackerDataOffline
                    {
                        TrackerId = Guid.Parse(trackerDataElement.Element("TrackerId")?.Value),
                        Date = DateTime.Parse(trackerDataElement.Element("Date")?.Value),
                        TotalTime = TimeSpan.Parse(trackerDataElement.Element("TotalTime")?.Value),
                        IdleTime = TimeSpan.Parse(trackerDataElement.Element("IdleTime")?.Value),
                        EmployeeUsername = trackerDataElement.Element("EmployeeUsername")?.Value
                    };
                }
                else
                {
                    return null;
                }
            }
            else { return null; }
        }
        public void RemoveOfflineTrackerData()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
        #endregion
    }
}