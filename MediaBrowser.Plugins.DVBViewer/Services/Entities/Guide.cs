﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

namespace MediaBrowser.Plugins.DVBViewer.Services.Entities
{
    [XmlRoot("epg")]
    public class Guide
    {
        [XmlElement("programme")]
        public List<Program> Program { get; set; }

        [XmlAttribute("Ver")]
        public string Version { get; set; }
    }

    public class Program
    {
        [XmlElement("titles", IsNullable = true)]
        public Titles Titles { private get; set; }

        [XmlElement("title"), DefaultValue("")]
        public string Title { private get; set; }

        public string Name
        {
            get
            {
                if (Titles != null)
                {
                    return Titles.Title;
                }
                return Title;
            }
            set
            {
                Name = value;
            }
        }

        int productionYear;
        public int? ProductionYear
        {
            get
            {
                if (!String.IsNullOrEmpty(Name))
                {
                    if (Int32.TryParse((Regex.Match(Name, @"(?<=\()\d{4}(?=\)$)").Value), out productionYear))
                    {
                        return productionYear;
                    }
                }
                return null;
            }
            set
            {
                ProductionYear = value;
            }
        }


        [XmlElement("events", IsNullable = true)]
        public Events Events { private get; set; }

        [XmlElement("event"), DefaultValue("")]
        public string Event { private get; set; }

        public string EpisodeTitle
        {
            get
            {
                if (Events != null)
                {
                    if (!String.IsNullOrEmpty(Events.Event))
                    {
                        return Regex.Replace(Events.Event, @"(^[s]?[0-9]*[e|x|\.][0-9]*[^\w]+)|(\s[\(]?[s]?[0-9]*[e|x|\.][0-9]*[\)]?$)", String.Empty, RegexOptions.IgnoreCase);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(Event))
                    {
                        return Regex.Replace(Event, @"(^[s]?[0-9]*[e|x|\.][0-9]*[^\w]+)|(\s[\(]?[s]?[0-9]*[e|x|\.][0-9]*[\)]?$)", String.Empty, RegexOptions.IgnoreCase);
                    }
                }
                return null;
            }
            set
            {
                EpisodeTitle = value;
            }
        }


        int episodeNumber;
        public int? EpisodeNumber
        {
            get
            {
                if (Events != null)
                {
                    if (!String.IsNullOrEmpty(Events.Event))
                    {
                        if (Int32.TryParse(Regex.Match(Regex.Match(Events.Event, @"(?<=[s]?[0-9]+)[e|x|\.][0-9]+\s", RegexOptions.IgnoreCase).Value, @"\d+").Value, out episodeNumber))
                        {
                            return episodeNumber;
                        }
                    }
                    return null;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Event))
                    {
                        if (Int32.TryParse(Regex.Match(Regex.Match(Event, @"(?<=[s]?[0-9]+)[e|x|\.][0-9]+\s", RegexOptions.IgnoreCase).Value, @"\d+").Value, out episodeNumber))
                        {
                            return episodeNumber;
                        }
                    }
                    return null;
                }
            }
            set
            {
                EpisodeNumber = value;
            }
        }

        int seasonNumber;
        public int? SeasonNumber
        {
            get
            {
                if (Events != null)
                {
                    if (!String.IsNullOrEmpty(Events.Event))
                    {
                        if (Int32.TryParse(Regex.Match(Regex.Match(Events.Event, @"[s]?[0-9]+(?=[e|x|\.][0-9]+\s)", RegexOptions.IgnoreCase).Value, @"\d+").Value, out seasonNumber))
                        {
                            return seasonNumber;
                        }
                    }
                    return null;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Event))
                    {
                        if (Int32.TryParse(Regex.Match(Regex.Match(Event, @"[s]?[0-9]+(?=[e|x|\.][0-9]+\s)", RegexOptions.IgnoreCase).Value, @"\d+").Value, out seasonNumber))
                        {
                            return seasonNumber;
                        }
                    }
                    return null;
                }
            }
            set
            {
                SeasonNumber = value;
            }
        }


        [XmlElement("descriptions", IsNullable = true)]
        public Descriptions Descriptions { private get; set; }

        [XmlElement("description"), DefaultValue("")]
        public string Description { private get; set; }

        public string Overview
        {
            get
            {
                if (Descriptions != null)
                {
                    return Descriptions.Description;
                }
                return Description;
            }
            set
            {
                Overview = value;
            }
        }


        [XmlElement("eventid"), DefaultValue("")]
        public string EventId { get; set; }

        [XmlElement("content"), DefaultValue("")]
        public string EitContent { get; set; }

        [XmlElement("charset"), DefaultValue("")]
        public string Charset { get; set; }

        [XmlAttribute("start")]
        public string Start { get; set; }

        [XmlAttribute("stop")]
        public string Stop { get; set; }


        [XmlAttribute("channel")]
        public string ChannelEPGID { get; set; }

        public string ChannelId
        {
            get
            {
                if (ChannelEPGID != null)
                {
                    try
                    {
                        return Plugin.TvProxy.GetChannelList(new CancellationToken()).Root.ChannelGroup.SelectMany(c => c.Channel)
                        .Where(x => x.EPGID.Equals(ChannelEPGID))
                        .First().Id;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return null;
            }
            set
            {
                ChannelId = value;
            }
        }
    }

    public class Titles
    {
        [XmlElement("title")]
        public string Title { get; set; }
    }

    public class Events
    {
        [XmlElement("event"), DefaultValue("")]
        public string Event { get; set; }
    }

    public class Descriptions
    {
        [XmlElement("description"), DefaultValue("")]
        public string Description { get; set; }
    }
}