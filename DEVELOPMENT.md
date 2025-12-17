# Development Setup Guide

This document describes the development tools and setup for the AspireReact project.

---

## Prerequisites

### Required Tools

- **.NET SDK 10+** ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- **Node.js 18+** ([Download](https://nodejs.org/))
- **Docker Desktop** ([Download](https://www.docker.com/products/docker-desktop))
- **.NET Aspire Workload**

        ```bash
dotnet workload update
dotnet workload install aspire
```

### Recommended IDE

**Visual Studio Code** with the following extensions (auto-recommended via `.vscode/extensions.json`):

#### Essential Extensions

1. **Marksman** (`artempyanykh.marksman`)
   - Language server for Markdown linting, auto-completion, and link validation
   - Automatically enabled for `.md` files
   - Configuration: `.marksman.toml`

2. **C# Dev Kit** (`ms-dotnettools.csdevkit`)
   - Full C# development support
   - IntelliSense, debugging, testing

3. **.NET Aspire** (`ms-dotnettools.dotnet-aspire`)
   - Aspire project support and tooling

4. **ESLint** (`dbaeumer.vscode-eslint`)
   - JavaScript/TypeScript linting for React frontend

5. **Prettier** (`esbenp.prettier-vscode`)
   - Code formatting for TypeScript/React

When you open the project in VS Code, you'll be prompted to install recommended extensions.

---

## Markdown Linting with Marksman

This project uses [Marksman](https://github.com/artempyanykh/marksman) for Markdown documentation quality.

### What Marksman Provides

- ✅ **Link Validation**: Detects broken internal links (files, headings)
- ✅ **Auto-Completion**: Suggests file names, headings, and references
- ✅ **Go-to-Definition**: Navigate to linked files and sections
- ✅ **Diagnostics**: Real-time linting of Markdown syntax
- ✅ **Refactoring**: Rename files and update all references

### Installation

#### Option 1: VS Code Extension (Recommended)

1. Install the [Marksman extension](https://marketplace.visualstudio.com/items?itemName=artempyanykh.marksman)
2. Open a Markdown file - Marksman activates automatically

#### Option 2: Standalone Language Server

```bash
# macOS (Homebrew)
brew install marksman

# Linux
# Download from: https://github.com/artempyanykh/marksman/releases

# Windows
# Download from: https://github.com/artempyanykh/marksman/releases
```

### Configuration

Marksman is configured via `.marksman.toml`:

```toml
[core.diagnostics]
enabled = true  # Enable linting

[core.completion]
enabled = true  # Enable auto-completion

[core.markdown]
heading.style = "atx"  # Use # Heading style
```

### Usage Examples

#### Check Broken Links

Marksman automatically highlights broken links in real-time:

```markdown
❌ [Broken Link](./non-existent-file.md)
✅ [Working Link](./README.md)
✅ [Heading Link](#development-setup-guide)
```

#### Auto-Complete File Links

Type `[` and start typing a filename to see suggestions:

```markdown
[See the ADR](./ <-- Marksman suggests files here
```

#### Navigate to Definitions

- **Ctrl+Click** (Cmd+Click on Mac) on a link to open the target file
- **F12** to go to definition
- **Shift+F12** to peek definition

---

## Project Structure Linting

### Markdown Files

The following Markdown files are validated by Marksman:

- `README.md` - Main project documentation
- `ADR_ASPIRE_MIGRATION_9_TO_13.md` - Architecture decision record
- `ASPIRE_VERSION_COMPARISON.md` - Version comparison guide
- `DEVELOPMENT.md` - This file

### Link Validation

Marksman validates:
- ✅ Internal file links (e.g., `[ADR](./ADR_ASPIRE_MIGRATION_9_TO_13.md)`)
- ✅ Heading anchors (e.g., `[Section](#-getting-started)`)
- ✅ Reference-style links
- ⚠️ External links (warns if unreachable, doesn't fail)

### Common Issues and Fixes

#### Broken Heading Link

```markdown
❌ [Link](#non-existent-heading)
✅ [Link](#-architecture-decision-records)  # Must match exact heading ID
```

Tip: Hover over a heading to see its generated ID.

#### Case-Sensitive Links (Linux)

```markdown
# macOS/Windows (case-insensitive) - works
[Link](./README.md)
[Link](./readme.md)

# Linux (case-sensitive) - second one breaks
[Link](./README.md)  ✅
[Link](./readme.md)  ❌
```

Always use exact case for filenames.

---

## Development Workflow

### 1. Initial Setup

```bash
# Clone the repository
git clone <repository-url>
cd AspireReact

# Install .NET dependencies
dotnet restore

# Install frontend dependencies
cd AspireReact.React
npm install
cd ..
```

### 2. Open in VS Code

```bash
code .
```

VS Code will prompt you to:
1. Install recommended extensions (including Marksman)
2. Trust the workspace
3. Restore .NET dependencies

### 3. Run the Application

```bash
# From repository root
cd AspireReact.AppHost
dotnet run
```

This starts:
- ✅ API (auto-assigned port)
- ✅ React Frontend (auto-assigned port)
- ✅ Aspire Dashboard (https://localhost:17178)

### 4. Edit Markdown Documentation

When editing `.md` files:
- Marksman provides real-time diagnostics
- Broken links appear with squiggly underlines
- Auto-complete suggests file names and headings
- Ctrl+Click navigates to linked sections

---

## VS Code Settings

Project-specific settings are in `.vscode/settings.json`:

### Markdown Settings

```json
{
  "[markdown]": {
    "editor.wordWrap": "on",           // Wrap long lines
    "editor.quickSuggestions": true    // Auto-suggest as you type
  },

  "marksman.enabled": true,            // Enable Marksman
  "marksman.diagnostics.enabled": true // Show linting errors
}
```

### .NET Settings

```json
{
  "dotnet.defaultSolution": "AspireReact.slnx",  // Default solution file
  "files.exclude": {
    "**/bin": true,     // Hide build output
    "**/obj": true      // Hide intermediate files
  }
}
```

---

## Code Quality Checks

### Before Committing

1. **Check Markdown Links**
   - Open all `.md` files in VS Code
   - Verify no broken link warnings from Marksman
   - Run: `Ctrl+Shift+M` (Cmd+Shift+M on Mac) to see all diagnostics

2. **Build .NET Projects**
   ```bash
   dotnet build
   ```

3. **Lint Frontend**
   ```bash
   cd AspireReact.React
   npm run lint
   ```

4. **Format Code**
   ```bash
   # .NET (if using dotnet-format)
   dotnet format

   # React
   cd AspireReact.React
   npm run format
   ```

---

## Troubleshooting

### Marksman Not Working

**Issue**: No diagnostics or auto-completion in Markdown files

**Solutions**:
1. Check extension is installed: `Ctrl+Shift+X` → Search "Marksman"
2. Verify `.marksman.toml` exists in repository root
3. Reload VS Code: `Ctrl+Shift+P` → "Developer: Reload Window"
4. Check Output panel: View → Output → Select "Marksman" from dropdown

### Broken Link False Positive

**Issue**: Marksman reports a link as broken but it works in browser

**Cause**: Case sensitivity or special characters in filenames

**Solution**: Use exact filename with correct case

---

## Additional Resources

### Marksman Documentation
- [GitHub Repository](https://github.com/artempyanykh/marksman)
- [VS Code Extension](https://marketplace.visualstudio.com/items?itemName=artempyanykh.marksman)
- [Language Server Protocol](https://microsoft.github.io/language-server-protocol/)

### .NET Aspire Development
- [Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Samples](https://github.com/dotnet/aspire-samples)

### OpenTelemetry
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [Aspire Dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard)

---

## Contributing

See [README.md](./README.md#-contributing) for contribution guidelines.

### Documentation Standards

When contributing to Markdown documentation:

1. **Use Marksman** to validate links before committing
2. **Follow existing structure** (headings, code blocks, diagrams)
3. **Add ADRs** for significant architectural decisions
4. **Update cross-references** when renaming files or sections
5. **Test all code examples** in code blocks

---

## License

This project is provided as-is for educational and demonstration purposes.
