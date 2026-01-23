using System;
using System.Collections.Generic;

namespace Churchee.Common.Extensibility
{
    public class MenuItem
    {
        public MenuItem(string name, string path, string icon, int order = 1, string reqiredRole = "")
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentException.ThrowIfNullOrEmpty(path);
            ArgumentException.ThrowIfNullOrEmpty(icon);

            if (!path.StartsWith("/management"))
            {
                throw new FormatException("Path must start with /management");
            }

            Name = name;
            Path = path;
            Icon = icon;
            Children = [];
            Order = order;
            RequiredRole = reqiredRole;
        }

        public MenuItem(string name, string path, string icon, List<MenuItem> children)
        {
            Name = name;
            Path = path;
            Icon = icon;
            Children = children;
            Order = 1;
        }

        public int Order { get; }

        public string Name { get; }

        public string Path { get; }

        public string Icon { get; }

        public string RequiredRole { get; }

        public List<MenuItem> Children { get; }

        public MenuItem AddChild(MenuItem menu)
        {
            Children.Add(menu);

            return this;
        }
    }
}
