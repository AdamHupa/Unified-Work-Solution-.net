using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary.DbModels.Helpers
{
    public static class TriggerHelpers
    {
        public static string Drop(string trigger)
        {
            return "DROP TRIGGER [" + trigger + "] ";
        }

        public static string PostUpdateSetter_UtcDate(string trigger, string table, string idColumn, string modifiedColumn)
        {
            return "CREATE TRIGGER [" + trigger + "] " +
                   "ON [" + table + "] AFTER UPDATE AS " +
                   "UPDATE [" + table + "] SET  [" + modifiedColumn + "] = GETUTCDATE() " +
                   "WHERE [" + idColumn + "] in (select [" + idColumn + "] from inserted) " +
                   "ALTER TABLE [" + table + "] ENABLE TRIGGER [" + trigger + "] ";
        }

        public static string PostUpdateSetter_UtcDate(string trigger, string table, string idColumn1, string idColumn2, string modifiedColumn)
        {
            return "CREATE TRIGGER [" + trigger + "] " +
                   "ON [" + table + "] AFTER UPDATE AS " +
                   "UPDATE [" + table + "] SET  [" + modifiedColumn + "] = GETUTCDATE() " +
                   "WHERE [" + idColumn1 + "] = (SELECT TOP 1 [" + idColumn1 + "] FROM inserted) " +
                   "  AND [" + idColumn2 + "] = (SELECT top 1 [" + idColumn2 + "] FROM inserted) " +
                   "ALTER TABLE [" + table + "] ENABLE TRIGGER [" + trigger + "] ";
        }

        public static string PostUpdateSetter_UtcDate(string trigger, string table, string idColumn1, string idColumn2, string idColumn3, string modifiedColumn)
        {
            return "CREATE TRIGGER [" + trigger + "] " +
                   "ON [" + table + "] AFTER UPDATE AS " +
                   "UPDATE [" + table + "] SET  [" + modifiedColumn + "] = GETUTCDATE() " +
                   "WHERE [" + idColumn1 + "] = (SELECT TOP 1 [" + idColumn1 + "] FROM inserted) " +
                   "  AND [" + idColumn2 + "] = (SELECT top 1 [" + idColumn2 + "] FROM inserted) " +
                   "  AND [" + idColumn3 + "] = (SELECT top 1 [" + idColumn3 + "] FROM inserted) " +
                   "ALTER TABLE [" + table + "] ENABLE TRIGGER [" + trigger + "] ";
        }
    }
}
