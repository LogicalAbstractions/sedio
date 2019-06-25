namespace Sedio.Core.Application
{
    public interface IApplication
    {
        string RootPath { get; }

        bool IsProduction { get; }
    }
}