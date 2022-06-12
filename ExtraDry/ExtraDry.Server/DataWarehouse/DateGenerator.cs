﻿using ExtraDry.Server.DataWarehouse.Builder;
using ExtraDry.Server.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ExtraDry.Server.DataWarehouse;

public class DateGenerator : IDataGenerator {

    public DateGenerator(Action<DateGeneratorOptions>? options = null)
    {
        OptionsBuilder = options;
        RefreshOptions();
    }

    [JsonIgnore]
    public DateOnly StartDate => Options.StartDate;

    [JsonIgnore] 
    public DateOnly EndDate => Options.EndDate;

    public int FiscalYearEndingMonth => Options.FiscalYearEndingMonth;

    public async Task<List<object>> GetBatchAsync(Table table, DbContext oltp, DbContext olap, ISqlGenerator sql)
    {
        RefreshOptions();
        var batch = new List<object>();

        var minSql = sql.SelectMinimum(table, nameof(Date.Sequence));
        var actualMin = await olap.Database.ExecuteScalerAsync(minSql);
        var requiredMin = DateToInt(StartDate);
        if(requiredMin < actualMin) {
            // Earlier dates are required, ensure they're added in decreasing order, reverses typical for loop.
            var start = actualMin - 1;
            var end = Math.Max(requiredMin, start - 100);
            for(int d = start; d >= end; --d) {
                AddDatesToBatch(batch, d);
            }
            return batch;
        }

        var maxSql = sql.SelectMaximum(table, nameof(Date.Sequence));
        var actualMax = await olap.Database.ExecuteScalerAsync(maxSql);

        var requiredMax = DateToInt(EndDate);
        if(requiredMax > actualMax) {
            var start = Math.Max(DateToInt(StartDate), actualMax + 1);
            var end = Math.Min(requiredMax, start + 100);
            for(int d = start; d < end; ++d) {
                AddDatesToBatch(batch, d);
            }
            return batch;
        }

        return batch;
    }

    private void AddDatesToBatch(List<object> batch, int d)
    {
        var date = IntToDate(d);
        foreach(var day in Options.DayTypesSelector(date)) {
            batch.Add(new Date(d, day) { FiscalYearEndingMonth = FiscalYearEndingMonth });
        }
    }

    public DateTime GetSyncTimestamp() => DateTime.UtcNow;

    private void RefreshOptions() => OptionsBuilder?.Invoke(Options);

    private static int DateToInt(DateOnly date) => (date.ToDateTime(new TimeOnly(0)) - DateTime.UnixEpoch).Days;

    private static DateOnly IntToDate(int days) => new DateOnly(1970, 1, 1).AddDays(days);

    private DateGeneratorOptions Options { get; set; } = new DateGeneratorOptions();

    private Action<DateGeneratorOptions>? OptionsBuilder { get; set; }

}
