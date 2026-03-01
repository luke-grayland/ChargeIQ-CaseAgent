# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# All commands run from the ChargeIQ-CaseAgent/ directory
dotnet restore
dotnet build
dotnet run --project CaseAgent    # http://localhost:5265 / https://localhost:7204
```

No unit tests configured. Interactive testing via Swagger UI in development mode.

## Architecture

.NET 8 ASP.NET Core API that uses OpenAI's Chat API to generate chargeback documentation. Takes dispute case data, validates it against Mastercom guidelines, and produces PDF output.

### Request Flow

```
ChargebackController
  → ChargebackGenerationService (orchestrates OpenAI chat with tool calls)
    → ToolsResponseHandler (dispatches tool call results back to OpenAI)
    → ChargebackValidator (validates case data against Mastercom rules)
    → PdfGenerationService (renders final PDF)
```

### Key Components

- **`Prompts/`** — Text prompt templates loaded at runtime by `PromptLoaderService`. Includes `FirstChargebackGenerationPrompt.txt` and `MastercomLLMDoc.txt` (Mastercom API reference).
- **`Tools/`** — OpenAI function-calling tools: `MastercomGuidelinesTool` provides Mastercom knowledge, `Tools.cs` defines the tool schemas.
- **`Services/`** — `ChargebackGenerationService` runs the multi-turn OpenAI conversation with tool use; `ChargebackValidator` checks case validity; `PdfGenerationService` produces PDF output.

### Configuration

Required in `appsettings.json` or environment:
- `OPENAI_API_KEY` — OpenAI API key
- `MODEL` — OpenAI model name (e.g. `gpt-4o`)
