
using System;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Voice.Protocols.TServer.Events;
namespace Pointel.Integration.Core.Helper
{
  public  class MediaEventHelper
    {
     // public long// ID = 0;

      public bool ConvertVoiceEvent(ref Type objType,ref object obj,ref KeyValueCollection userData,IMessage objEvent)
      {
         // ID = 0;
          if (objEvent!=null)
          switch (objEvent.Id)
          {
              case EventRinging.MessageId:
                  EventRinging eventRinging = (EventRinging)objEvent;
                 // ID =eventRinging.ConnID.ToLong();
                  objType = eventRinging.GetType();
                  obj = eventRinging;
                  userData = eventRinging.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventRinging.ConnID.ToString());
                  break;
              case EventReleased.MessageId:
                  EventReleased eventReleased = (EventReleased)objEvent;
                 // ID = eventReleased.ConnID.ToString().ToLong();
                  objType = eventReleased.GetType();
                  obj = eventReleased;
                  userData = eventReleased.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventReleased.ConnID.ToString());
                  break;

              case EventEstablished.MessageId:
                  EventEstablished eventEstablished = (EventEstablished)objEvent;
                 // ID = eventEstablished.ConnID.ToString().ToLong();
                  objType = eventEstablished.GetType();
                  obj = eventEstablished;
                  userData = eventEstablished.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventEstablished.ConnID.ToString());
                  break;

              case EventHeld.MessageId:
                  EventHeld eventHeld = (EventHeld)objEvent;
                 // ID = eventHeld.ConnID.ToString().ToLong();
                  objType = eventHeld.GetType();
                  obj = eventHeld;
                  userData = eventHeld.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventHeld.ConnID.ToString());
                  break;

              case EventPartyChanged.MessageId:
                  EventPartyChanged eventPartyChanged = (EventPartyChanged)objEvent;
                 // ID = eventPartyChanged.ConnID.ToString().ToLong();
                  objType = eventPartyChanged.GetType();
                  obj = eventPartyChanged;
                  userData = eventPartyChanged.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventPartyChanged.ConnID.ToString());
                  break;

              case EventAttachedDataChanged.MessageId:
                  EventAttachedDataChanged eventAttachedDataChanged = (EventAttachedDataChanged)objEvent;
                 // ID = eventAttachedDataChanged.ConnID.ToString().ToLong();
                  objType = eventAttachedDataChanged.GetType();
                  obj = eventAttachedDataChanged;
                  userData = eventAttachedDataChanged.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventAttachedDataChanged.ConnID.ToString());
                  break;

              case EventDialing.MessageId:
                  EventDialing eventDialing = (EventDialing)objEvent;
                 // ID = eventDialing.ConnID.ToString().ToLong();
                  objType = eventDialing.GetType();
                  obj = eventDialing;
                  userData = eventDialing.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventDialing.ConnID.ToString());
                  break;

              case EventRetrieved.MessageId:
                  EventRetrieved eventRetrieved = (EventRetrieved)objEvent;
                 // ID = eventRetrieved.ConnID.ToString().ToLong();
                  objType = eventRetrieved.GetType();
                  obj = eventRetrieved;
                  userData = eventRetrieved.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventRetrieved.ConnID.ToString());
                  break;

              case EventAbandoned.MessageId:
                  EventAbandoned eventAbandoned = (EventAbandoned)objEvent;
                 // ID = eventAbandoned.ConnID.ToString().ToLong();
                  objType = eventAbandoned.GetType();
                  obj = eventAbandoned;
                  userData = eventAbandoned.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventAbandoned.ConnID.ToString());
                  break;

              case EventPartyAdded.MessageId:
                  EventAbandoned eventPartyAdded = (EventAbandoned)objEvent;
                 // ID = eventPartyAdded.ConnID.ToString().ToLong();
                  objType = eventPartyAdded.GetType();
                  obj = eventPartyAdded;
                  userData = eventPartyAdded.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventPartyAdded.ConnID.ToString());
                  break;

              case EventPartyDeleted.MessageId:
                  EventPartyDeleted eventPartyDeleted = (EventPartyDeleted)objEvent;
                 // ID = eventPartyDeleted.ConnID.ToString().ToLong();
                  objType = eventPartyDeleted.GetType();
                  obj = eventPartyDeleted;
                  userData = eventPartyDeleted.UserData;
                  if (userData != null && !userData.ContainsKey("ConnectionId"))
                      userData.Add("ConnectionId", eventPartyDeleted.ConnID.ToString());
                  break;
          }
          return obj != null && objType != null;
      }

      public bool ConvertEmailEvent(ref Type objType, ref object obj, ref KeyValueCollection userData, IMessage objEvent)
      {
         // ID = 0;
          if (objEvent != null)
              switch (objEvent.Id)
              {
                  case EventRinging.MessageId:
                      EventRinging eventRinging = (EventRinging)objEvent;
                      objType = eventRinging.GetType();
                      obj = eventRinging;
                      userData = eventRinging.UserData;
                      break;
                  case EventReleased.MessageId:
                      EventReleased eventReleased = (EventReleased)objEvent;
                      objType = eventReleased.GetType();
                      obj = eventReleased;
                      userData = eventReleased.UserData;
                      break;

                  case EventEstablished.MessageId:
                      EventEstablished eventEstablished = (EventEstablished)objEvent;
                      objType = eventEstablished.GetType();
                      obj = eventEstablished;
                      userData = eventEstablished.UserData;
                      break;

                  case EventHeld.MessageId:
                      EventHeld eventHeld = (EventHeld)objEvent;
                      objType = eventHeld.GetType();
                      obj = eventHeld;
                      userData = eventHeld.UserData;
                      break;

                  case EventPartyChanged.MessageId:
                      EventPartyChanged eventPartyChanged = (EventPartyChanged)objEvent;
                      objType = eventPartyChanged.GetType();
                      obj = eventPartyChanged;
                      userData = eventPartyChanged.UserData;
                      break;

                  case EventAttachedDataChanged.MessageId:
                      EventAttachedDataChanged eventAttachedDataChanged = (EventAttachedDataChanged)objEvent;
                      objType = eventAttachedDataChanged.GetType();
                      obj = eventAttachedDataChanged;
                      userData = eventAttachedDataChanged.UserData;
                      break;

                  case EventDialing.MessageId:
                      EventDialing eventDialing = (EventDialing)objEvent;
                      objType = eventDialing.GetType();
                      obj = eventDialing;
                      userData = eventDialing.UserData;
                      break;

                  case EventRetrieved.MessageId:
                      EventRetrieved eventRetrieved = (EventRetrieved)objEvent;
                      objType = eventRetrieved.GetType();
                      obj = eventRetrieved;
                      userData = eventRetrieved.UserData;
                      break;

                  case EventAbandoned.MessageId:
                      EventAbandoned eventAbandoned = (EventAbandoned)objEvent;
                      objType = eventAbandoned.GetType();
                      obj = eventAbandoned;
                      userData = eventAbandoned.UserData;
                      break;

                  case EventPartyAdded.MessageId:
                      EventAbandoned eventPartyAdded = (EventAbandoned)objEvent;
                      objType = eventPartyAdded.GetType();
                      obj = eventPartyAdded;
                      userData = eventPartyAdded.UserData;
                      break;

                  case EventPartyDeleted.MessageId:
                      EventPartyDeleted eventPartyDeleted = (EventPartyDeleted)objEvent;
                      objType = eventPartyDeleted.GetType();
                      obj = eventPartyDeleted;
                      userData = eventPartyDeleted.UserData;
                      break;
              }
          return obj != null && objType != null;
      }

      public bool ConvertChatEvent(ref Type objType, ref object obj, ref KeyValueCollection userData, IMessage objEvent)
      {
         // ID = 0;
          if (objEvent != null)
              switch (objEvent.Id)
              {
                  case EventRinging.MessageId:
                      EventRinging eventRinging = (EventRinging)objEvent;
                      objType = eventRinging.GetType();
                      obj = eventRinging;
                      userData = eventRinging.UserData;
                      break;
                  case EventReleased.MessageId:
                      EventReleased eventReleased = (EventReleased)objEvent;
                      objType = eventReleased.GetType();
                      obj = eventReleased;
                      userData = eventReleased.UserData;
                      break;

                  case EventEstablished.MessageId:
                      EventEstablished eventEstablished = (EventEstablished)objEvent;
                      objType = eventEstablished.GetType();
                      obj = eventEstablished;
                      userData = eventEstablished.UserData;
                      break;

                  case EventHeld.MessageId:
                      EventHeld eventHeld = (EventHeld)objEvent;
                      objType = eventHeld.GetType();
                      obj = eventHeld;
                      userData = eventHeld.UserData;
                      break;

                  case EventPartyChanged.MessageId:
                      EventPartyChanged eventPartyChanged = (EventPartyChanged)objEvent;
                      objType = eventPartyChanged.GetType();
                      obj = eventPartyChanged;
                      userData = eventPartyChanged.UserData;
                      break;

                  case EventAttachedDataChanged.MessageId:
                      EventAttachedDataChanged eventAttachedDataChanged = (EventAttachedDataChanged)objEvent;
                      objType = eventAttachedDataChanged.GetType();
                      obj = eventAttachedDataChanged;
                      userData = eventAttachedDataChanged.UserData;
                      break;

                  case EventDialing.MessageId:
                      EventDialing eventDialing = (EventDialing)objEvent;
                      objType = eventDialing.GetType();
                      obj = eventDialing;
                      userData = eventDialing.UserData;
                      break;

                  case EventRetrieved.MessageId:
                      EventRetrieved eventRetrieved = (EventRetrieved)objEvent;
                      objType = eventRetrieved.GetType();
                      obj = eventRetrieved;
                      userData = eventRetrieved.UserData;
                      break;

                  case EventAbandoned.MessageId:
                      EventAbandoned eventAbandoned = (EventAbandoned)objEvent;
                      objType = eventAbandoned.GetType();
                      obj = eventAbandoned;
                      userData = eventAbandoned.UserData;
                      break;

                  case EventPartyAdded.MessageId:
                      EventAbandoned eventPartyAdded = (EventAbandoned)objEvent;
                      objType = eventPartyAdded.GetType();
                      obj = eventPartyAdded;
                      userData = eventPartyAdded.UserData;
                      break;

                  case EventPartyDeleted.MessageId:
                      EventPartyDeleted eventPartyDeleted = (EventPartyDeleted)objEvent;
                      objType = eventPartyDeleted.GetType();
                      obj = eventPartyDeleted;
                      userData = eventPartyDeleted.UserData;
                      break;
              }
          return obj != null && objType != null;
      }
    }
}

