using System;
using System.IO;

namespace XLN.Game.Common
{
    public struct ResourcePath
    {
        public enum PathType
        {
            Resource,
            File,
            HTTP,
            Any
        }

        public enum FileType
        {
            SINGLE,
            BUNDLE
        }


        public ResourcePath(PathType type, string path)
            : this(path)
        {
            m_Type = type;
        }

        public ResourcePath(string path)
        {
            m_Type = PathType.Resource;
            m_Path = path;
            m_SubFile = null;
            m_ResourceType = FileType.SINGLE;
        }

        public ResourcePath(PathType type, string path, string subFile)
            :this(path)
        {
            m_Type = type;
            m_SubFile = subFile;
            m_ResourceType = FileType.BUNDLE;
        }

        public string GetExt()
        {
            return System.IO.Path.GetExtension(m_Path);
        }

        public Uri GetUri()
        {
            UriBuilder builder = new UriBuilder();
            switch(m_Type)
            {
                case PathType.File:
                    builder.Scheme = "file";
                    break;
                case PathType.HTTP:
                    builder.Scheme = "http";
                    break;
                case PathType.Any:
                    builder.Scheme = "any";
                    break;
                default:
                    builder.Scheme = "resource";
                    break;

            } 
            return builder.Uri;
        }

        PathType m_Type;
        public PathType Type
        {
            get
            {
                return m_Type;
            }
        }

        string m_Path;

        public string Path
        {
            get
            {
                return m_Path;
            }
        }

        string m_SubFile;

        public string SubFile
        {
            get
            {
                return m_SubFile;
            }
        }

        FileType m_ResourceType;

        public FileType ResourceType
        {
            get
            {
                return m_ResourceType;
            }
        }
    }
}
