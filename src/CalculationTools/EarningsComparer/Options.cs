using System;
using System.Text;
using ClosedXML.Excel;
using CommandLine;
using CommandLine.Text;

namespace EarningsComparer
{
    internal enum FilterMode
    {
        None,
        Whitelist,
        Blacklist
    }
    


    internal class Options
    {
            [Option('c', "collectionPeriod", Required = true, HelpText = "The collection period for which to calculate earnings")]
            public short CollectionPeriod { get; set; }

            [Option('s', "startTime", Required = true, HelpText = "The start time of processing. This will be used to filter earnings from this date/time. Please provide this (in quotes) in the following format: \"2017-01-10 12:10:15\"")]
            public DateTime ProcessingStartTime { get; set; }

            [Option('f', "filterMode", HelpText = "valid options are \"None\" (Default), WhiteList or BlackList")]
            public FilterMode ProcessingFilterMode { get; set; }
        
    }
}