# Swinging Door

[TOC]

## Purpose

Data reduction by using the swinging door algorithm.

## Description

![](./images/swinging-door_02.png)

Beginning at the last archived value (1) and the next snapshots (2, 3, ...) a _swinging door_ is constructed,
that is only allowed to close and not to open. Green area in the figure below.

![](./images/swinging-door_01.png)

When an incoming value (6) lies outside the allowed aread, so the last snapshot and the new value get stored.
Therefore maintaining the trend in the data.

## Parameters

| Name | Description |
| -- | -- |
| CompDev | (absolut) compression deviation |
| ExMax | length of x/time before for sure a value gets recoreded |
| ExMin | length of x/time within no value gets recorded (after the last archived value) |

## Examples

### Trend

![](./images/swinging-door_trend.png)

### Max Delta

![](./images/swinging-door_maxDelta.png)

### Error and Statistics

![](./images/swinging-door_error.png)

| Data | # datapoints | average | sigma | skewness | kurtosis |
| -- | -- | -- | -- | -- | -- |
| raw | 1000 | 19.2854 | 1.2968 | -2.1689 | 7.0397 |
| compressed | 418 | 19.2833 | 1.2984 | -2.1682 | 7.0428 |

As can be seen statistics didn't change significantally, but the count of recorded datapoints was
reduced -- by filtering noise -- by 58%.

## Literature

* [OSIsoft: Exception and Compression Full Details](https://www.youtube.com/watch?v=89hg2mme7S0)
