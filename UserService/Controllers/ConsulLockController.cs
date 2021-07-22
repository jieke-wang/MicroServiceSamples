using System;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Mvc;
using UserService.ConsulLibs;

namespace UserService.Controllers {
    [ApiController]
    [Route ("UserService/[controller]/[action]")]
    public class ConsulLockController : ControllerBase {
        private readonly IConsulLock _lock;

        public ConsulLockController (IConsulLock @lock) {
            _lock = @lock;
        }

        [HttpGet]
        public async Task<int> LockDemo1Async (int stock) {
            int s = stock;

            // IDistributedLock stockLock = await _lock.AcquireLockAsync ("StockLock");
            // await stockLock.Release();

            try {
                ParallelLoopResult result = Parallel.For (1, stock + 1, i => {
                    // IDistributedLock stockLock = await _lock.AcquireLockAsync ("StockLock");
                    IDistributedLock stockLock = _lock.AcquireLockAsync ("StockLock").ConfigureAwait (false).GetAwaiter ().GetResult ();
                    // stockLock.Acquire ().ConfigureAwait (false).GetAwaiter ().GetResult ();
                    try {
                        Console.WriteLine ($"扣库存前: {i} => {s}");
                        s--;
                        Console.WriteLine ($"扣库存后: {i} => {s}");
                    } catch (System.Exception ex) {
                        Console.WriteLine (ex);
                    } finally {
                        stockLock.Release ().ConfigureAwait (false).GetAwaiter ().GetResult ();
                        // stockLock.Destroy ().ConfigureAwait (false).GetAwaiter ().GetResult ();
                    }
                });

                while (result.IsCompleted == false) {
                    await Task.Delay (10);
                }
            } catch (Exception ex) {
                Console.WriteLine (ex);
            } finally {
                // await stockLock.Destroy ();
            }

            return s;
        }

        // [HttpGet]
        // public async Task<int> LockDemo2Async (int stock) {
        //     int s = stock;
        //     IDistributedLock stockLock = _lock.CreateLock ("StockLock");

        //     try {
        //         ParallelLoopResult result = Parallel.For (1, stock + 1, i => {
        //             stockLock.Acquire ().ConfigureAwait (false).GetAwaiter ().GetResult ();
        //             try {
        //                 Console.WriteLine ($"扣库存前: {i} => {s}");
        //                 s--;
        //                 Console.WriteLine ($"扣库存后: {i} => {s}");
        //             } catch (Exception ex) {
        //                 Console.WriteLine (ex);
        //             } finally {
        //                 stockLock.Release ();
        //             }
        //         });

        //         while (result.IsCompleted == false) {
        //             await Task.Delay (10);
        //         }
        //     } catch (Exception ex) {
        //         Console.WriteLine (ex);
        //     } finally {
        //         // await stockLock.Destroy ();
        //     }

        //     return s;
        // }
    }
}