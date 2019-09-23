using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public static class Extensions
    {
        //public static async Task AddCompletedMessage(this IJobStorageService jobStorageService, Guid id, CancellationToken cancellationToken)
        //{
        //    var completedMessages = await jobStorageService.GetCompletedMessageIdentifiers(cancellationToken).ConfigureAwait(false);
        //    if (cancellationToken.IsCancellationRequested)
        //        return;
        //    if (completedMessages.Contains(id))
        //        return;
        //    completedMessages.Add(id);
        //    await jobStorageService.StoreCompletedMessageIdentifiers(completedMessages, cancellationToken).ConfigureAwait(false);
        //}

        //public static async Task RemoveCompletedMessage(this IJobStorageService jobStorageService, Guid id, CancellationToken cancellationToken)
        //{
        //    var completedMessages = await jobStorageService.GetCompletedMessageIdentifiers(cancellationToken).ConfigureAwait(false);
        //    if (cancellationToken.IsCancellationRequested)
        //        return;
        //    if (!completedMessages.Contains(id))
        //        return;
        //    completedMessages.Remove(id);
        //    await jobStorageService.StoreCompletedMessageIdentifiers(completedMessages, cancellationToken).ConfigureAwait(false);
        //}

        //public static async Task RemoveCompletedMessages(this IJobStorageService jobStorageService, List<Guid> ids, CancellationToken cancellationToken)
        //{
        //    var completedMessages = await jobStorageService.GetCompletedMessageIdentifiers(cancellationToken).ConfigureAwait(false);
        //    if (cancellationToken.IsCancellationRequested)
        //        return;
        //    completedMessages.RemoveAll(ids.Contains);
        //    await jobStorageService.StoreCompletedMessageIdentifiers(completedMessages, cancellationToken).ConfigureAwait(false);
        //}

        //public static async Task AddInProgressMessage(this IJobStorageService jobStorageService, Guid id, CancellationToken cancellationToken)
        //{
        //    var inProgressMessages = await jobStorageService.GetInProgressMessages(cancellationToken).ConfigureAwait(false);
        //    if (cancellationToken.IsCancellationRequested)
        //        return;
        //    if (inProgressMessages.Contains(id))
        //        return;
        //    inProgressMessages.Add(id);
        //    await jobStorageService.StoreInProgressMessages(inProgressMessages, cancellationToken).ConfigureAwait(false);
        //}

        //public static async Task AddInProgressMessages(this IJobStorageService jobStorageService, List<Guid> ids, CancellationToken cancellationToken)
        //{
        //    var inProgressMessages = await jobStorageService.GetInProgressMessages(cancellationToken).ConfigureAwait(false);
        //    if (cancellationToken.IsCancellationRequested)
        //        return;
        //    inProgressMessages.AddRange(ids.Where(id => !inProgressMessages.Contains(id)));
        //    await jobStorageService.StoreInProgressMessages(inProgressMessages, cancellationToken).ConfigureAwait(false);
        //}

        //public static async Task RemoveInProgressMessage(this IJobStorageService jobStorageService, Guid id, CancellationToken cancellationToken)
        //{
        //    var inProgressMessages = await jobStorageService.GetInProgressMessages(cancellationToken).ConfigureAwait(false);
        //    if (cancellationToken.IsCancellationRequested)
        //        return;
        //    if (!inProgressMessages.Contains(id))
        //        return;
        //    inProgressMessages.Remove(id);
        //    await jobStorageService.StoreInProgressMessages(inProgressMessages, cancellationToken).ConfigureAwait(false);
        //}

        //public static async Task<bool> HasInProgressMessage(this IJobStorageService jobStorageService, Guid id, CancellationToken cancellationToken)
        //{
        //    var inProgressMessages = await jobStorageService.GetInProgressMessages(cancellationToken).ConfigureAwait(false);
        //    return inProgressMessages.Contains(id);
        //}
    }
}