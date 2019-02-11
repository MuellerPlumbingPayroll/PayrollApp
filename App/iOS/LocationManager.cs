﻿using System;
using CoreLocation;

namespace Timecard.iOS
{
    public class LocationManager : CLLocationManagerDelegate
    {
        private readonly CLLocationManager Manager;

        public LocationManager()
        {
            Manager = new CLLocationManager
            {
                Delegate = this,
                // Keeping the accuarcy to 100 meters improves app performance,
                // device battery life, and reduces the chances of an error occurring.
                DesiredAccuracy = CLLocation.AccuracyHundredMeters
            };

            Manager.StartUpdatingLocation();
        }

        public void EnableLocationServices()
        {
            switch (CLLocationManager.Status)
            {
                case (CLAuthorizationStatus.NotDetermined):
                    Manager.RequestWhenInUseAuthorization();
                    break;
            }
        }

        public CLLocationCoordinate2D GetUserLocation()
        {
            if (CLLocationManager.Status != CLAuthorizationStatus.AuthorizedWhenInUse)
                throw new LocationNotAuthorizedException("Location services are not enabled.");
            try
            {
                return Manager.Location.Coordinate;
            }
            catch (Exception ex)
            {
                throw new LocationNotAuthorizedException("Location is currently unavailable.", ex);
            }
        }
    }
}
