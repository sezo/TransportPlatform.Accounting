using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportPlatform.Accounting.Application.Handlers;
using TransportPlatform.Accounting.Application.Queries;

namespace TransportPlatform.Accounting.Api.Controllers;

[ApiController]
[Route("api/ledger")]
[Authorize(Policy = "permission:accounting:read")]
public class LedgerController(GetLedgerHandler handler) : ControllerBase
{
    /// <summary>Get the current company balance and credit/debit totals.</summary>
    /// <remarks>
    /// Balance = sum of confirmed credits (ticket sales) minus sum of confirmed debits (refunds).
    /// Only confirmed ledger entries are included — pending and reversed entries are excluded.
    /// </remarks>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(LedgerSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSummary(CancellationToken ct)
    {
        var result = await handler.HandleSummaryAsync(new GetLedgerSummaryQuery(), ct);
        return Ok(result);
    }

    /// <summary>List all ledger entries (paged), newest first.</summary>
    /// <remarks>
    /// Each entry corresponds to a ticket transaction: a credit when a ticket is sold,
    /// a debit when payment is refunded after cancellation.
    /// </remarks>
    [HttpGet("entries")]
    [ProducesResponseType(typeof(PagedResult<LedgerEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEntries(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await handler.HandleEntriesAsync(new GetLedgerEntriesQuery(page, pageSize), ct);
        return Ok(result);
    }
}
