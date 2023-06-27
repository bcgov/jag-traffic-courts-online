
# Reference

[Using PromQL](https://docs.sysdig.com/en/docs/sysdig-monitor/dashboards/using-promql/#apply-a-dashboard-scope-to-a-promql-query)


# Compute the average duration of a histogram

Histogram has three metrics exported:

* *metric*_bucket - the histogram
* *metric*_count - A counter with the total number of measurements.
* *metric*_sum - A counter with the total number of measurements.

Given a histogram *metric* that reports the time an operation takes, the following query returns the average time
a operation tool in each interval. 

```promql
increase(metric_sum{$__scope}[$__interval]) / increase(metric_count{$__scope}[$__interval])
```

# Common Functions

* [histogram_quantile](https://prometheus.io/docs/prometheus/latest/querying/functions/#histogram_quantile)
* [increase](https://prometheus.io/docs/prometheus/latest/querying/functions/#increase) - calculates the increase in the time series in the range vector
* [rate](https://prometheus.io/docs/prometheus/latest/querying/functions/#rate) - calculates the per-second average rate of increase of the time series in the range vector
