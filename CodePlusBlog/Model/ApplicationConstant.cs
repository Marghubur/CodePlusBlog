﻿namespace CodePlusBlog.Model
{
    public static class ApplicationConstant
    {
        public const int PageSize = 9;
        public const int MinRandomValue = 10000;
        public const int MaxRandomValue = 999999999;
        public const string TempPassword = "CodePlus@123";
        
    }
    public enum Type
    {
        Article,
        Interview
    }

    public enum Category
    {
        Angular,
        CSharp,
        Javascript,
        Typescript,
        WebApi,
        DotnetCore,
        SqlServer,
        Mysql
    }
}
