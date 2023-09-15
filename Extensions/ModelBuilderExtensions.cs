using System;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Metadata;
using Bubblevel_MatchService.Models;

namespace Bubblevel_MatchService
{
  public static class ModelBuilderExtensions
  {
    public static void SeedData(this ModelBuilder modelBuilder)
    {
      #region Email Setting for send
      modelBuilder.Entity<EmailSetting>().HasData(new EmailSetting {
        Id = 1,
        Host = "smtp.gmail.com",
        Port = 465,
        Username = "bubblevel.services@gmail.com",
        Password = "epeq bxkr wjvh lnpj",
        UseSsl = true,
        UseTls = false
      });
      #endregion

      #region Email Config
      modelBuilder.Entity<Setting>().HasData(
        new Setting {
          Id = 1,
          State = State.Pending,
          DeliveryService = Emailing.CustomerOnly,
          EmailSender = null,
          Name = "Bubblevel",
          Summary = "Confirme Service",
          ResponseByEmail = true
        },
        new Setting {
          Id = 2,
          State = State.Rejected,
          DeliveryService = Emailing.Nothing,
          EmailSender = "bubblevel.services@gmail.com",
          Name = "Bubblevel",
          NameReceiver = null,
          Summary = null,
          ResponseByEmail = false
        },
        new Setting {
          Id = 3,
          State = State.OnHold,
          DeliveryService = Emailing.Internal,
          EmailSender = "bubblevel.services@gmail.com",
          Name = "Finance",
          NameReceiver = null,
          Summary = null,
          ResponseByEmail = true
        },
        new Setting {
          Id = 4,
          State = State.Awaiting,
          DeliveryService = Emailing.Internal,
          EmailSender = "bubblevel.services@gmail.com",
          Name = "Bubblevel",
          NameReceiver = null,
          Summary = null,
          ResponseByEmail = false
        },
        new Setting {
          Id = 5,
          State = State.InProgress,
          DeliveryService = Emailing.Nothing,
          EmailSender = null,
          Name = null,
          NameReceiver = null,
          Summary = null,
          ResponseByEmail = false
        },
        new Setting {
          Id = 6,
          State = State.Solved,
          DeliveryService = Emailing.CustomerOnly,
          EmailSender = null,
          Name = "Bubblevel",
          NameReceiver = null,
          Summary = null,
          ResponseByEmail = false
        }
      );
      #endregion
    }
  }
}

