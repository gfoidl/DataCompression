# Dead band

## Purpose

Filtering noise caused by measure- or device-errors (i.e. instrument precision).

## Description

Values that lie inbetween the dead band, defined by _ExDev_, get ommited because they are not meaningful. 
Decisions should not be based on these value. They are just noise, therefore they can be filtered out.

![](./images/DeadBand_01.jpg)

When a value is outside of the dead band, that value and the previous value are recored in order to maintain the trend.

![](./images/DeadBand_02.png)

## Parameters

| Name | Description |  
| -- | -- |  
| ExDev | (absolut) instrument precision |  
| ExMax | length of x/time before for sure a value gets recoreded |  

## Examples

![](./images/dead-band_trend.png)
![](./images/dead-band_maxDelta.png)

## Literature

* [OSIsoft: Exception and Compression Full Details](https://www.youtube.com/watch?v=89hg2mme7S0)