using System;
using System.Threading.Tasks;

namespace Sedio.Core.Threading
{
    public static class TaskCreationExtensions
    {
        public static Func<Task<TResult>> WrapInTask<TResult>(this Func<TResult> func)
        {
            return () =>
            {
                try
                {
                    return Task.FromResult(func.Invoke());
                }
                catch (Exception ex)
                {
                    return Task.FromException<TResult>(ex);
                }
            };
        }
        
        public static Func<TP1,Task<TResult>> WrapInTask<TResult,TP1>(this Func<TP1,TResult> func)
        {
            return (p1) =>
            {
                try
                {
                    return Task.FromResult(func.Invoke(p1));
                }
                catch (Exception ex)
                {
                    return Task.FromException<TResult>(ex);
                }
            };
        }
        
        public static Func<TP1,TP2,Task<TResult>> WrapInTask<TResult,TP1,TP2>(this Func<TP1,TP2,TResult> func)
        {
            return (p1,p2) =>
            {
                try
                {
                    return Task.FromResult(func.Invoke(p1,p2));
                }
                catch (Exception ex)
                {
                    return Task.FromException<TResult>(ex);
                }
            };
        }
        
        public static Func<TP1,TP2,TP3,Task<TResult>> WrapInTask<TResult,TP1,TP2,TP3>(this Func<TP1,TP2,TP3,TResult> func)
        {
            return (p1,p2,p3) =>
            {
                try
                {
                    return Task.FromResult(func.Invoke(p1,p2,p3));
                }
                catch (Exception ex)
                {
                    return Task.FromException<TResult>(ex);
                }
            };
        }
        
        public static Task<TResult> ExecuteAsTask<TResult>(this Func<TResult> func)
        {
            try
            {
                return Task.FromResult(func.Invoke());
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }
        
        public static Task<TResult> ExecuteAsTask<TResult,TP1>(this Func<TP1,TResult> func,TP1 p1)
        {
            try
            {
                return Task.FromResult(func.Invoke(p1));
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }
        
        public static Task<TResult> ExecuteAsTask<TResult,TP1,TP2>(this Func<TP1,TP2,TResult> func,TP1 p1,TP2 p2)
        {
            try
            {
                return Task.FromResult(func.Invoke(p1,p2));
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }
        
        public static Task<TResult> ExecuteAsTask<TResult,TP1,TP2,TP3>(this Func<TP1,TP2,TP3,TResult> func,TP1 p1,TP2 p2,TP3 p3)
        {
            try
            {
                return Task.FromResult(func.Invoke(p1,p2,p3));
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }
    }
}