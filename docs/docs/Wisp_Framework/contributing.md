---
icon: lucide/git-pull-request-arrow
---

# Contributor Guide

Thank you for your interest in contributing to Wisp Framework.

Wisp is a C# web framework built directly on the base .NET runtime rather than the ASP.NET Web SDK. It exists because there is room for experimentation, minimalism, and alternative approaches in the .NET ecosystem.

Contributions are welcome — but they need to be thoughtful.

## Why Contribute?

Open source software improves through real-world use and collaboration.

You might contribute because:

 - You found a bug and want it fixed.
 - You want to improve documentation.
 - You have an idea that aligns with Wisp’s design goals.
 - You want experience contributing to a framework.

All of those are valid reasons.

Wisp is intentionally small and opinionated. That means contributions can have real impact — but also that design decisions matter.

## Before You Open a Pull Request

### 1. Open an Issue First (for new features or larger changes)

If you want to:

 - Add a feature
 - Change behavior
 - Modify public APIs
 - Refactor significant internals

Please open an issue first on GitHub: [github.com/WispFramework/Wisp](https://github.com/WispFramework/Wisp)

Explain:

 - What you want to change
 - Why it should exist
 - What problem it solves
 - Describe the change and how to test it

This prevents wasted work and keeps the project direction coherent.

Small bug fixes and obvious documentation fixes can skip this step.

### 2. Pick "Good First Issue" if You're New

If this is your first contribution, start with issues labeled:

`good first issue`

These are intentionally scoped to help new contributors get familiar with the codebase.

### Code Standards

 - Follow existing style and structure and the [Common C# code conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
 - Avoid unnecessary abstractions.
 - Prefer clarity over cleverness.
 - Keep dependencies minimal. When adding a new dependency, consider using something that's already 
   available, or implementing the functionality yourself. [^1]
 - Don't change the formatting of existing files unless necessary. Avoid drive-by
   formatting changes.

### Low-Effort and LLM-Generated PRs

Pull requests that are:

 - Obviously AI-generated without understanding the codebase
 - Large mechanical rewrites with no clear benefit
 - Style-only churn
 - Speculative features without discussion
 - Refactors with no measurable improvement

…will be rejected.

Using AI as a tool is fine. Submitting unreviewed AI output is not.

If you cannot clearly explain your change and its impact, it is not ready to be merged.

### Tests and Stability

If you modify behavior:

 - Update or add tests if applicable.
 - Ensure the project builds cleanly.
 - Avoid breaking public APIs unless discussed in advance.

Stability matters.

Due to the rapidly evolving nature of Wisp at the moment, test coverage is still 
limited. Tests are not always required, but they are strongly encouraged.

### Documentation Changes

Documentation improvements are welcome and valuable.

 - Keep documentation factual and technical.
 - Avoid marketing language.
 - LLM-generated contributions to documentation are accepted, as long as
   they uphold the quality standards and are factually correct

Prefer examples over abstract descriptions.

### Code of Conduct

This project follows the [Contributor Covenant Code of Conduct](https://www.contributor-covenant.org/version/3/0/code_of_conduct/).

By participating, you agree to uphold its standards of respectful and inclusive behavior.

If you experience or witness unacceptable behavior, please report it through the appropriate GitHub channels.

### Design Philosophy

Wisp Framework aims to:

 - Stay lightweight
 - Avoid unnecessary magic
 - Keep abstractions understandable
 - Build on raw .NET primitives wherever possible
 - Remain small and hackable
 - Don't add significant new functionality to the base framework. Create an extension instead.

Contributions that increase complexity without strong justification are unlikely to be accepted.

If you're unsure whether something fits, ask first.

### Branching Model

- `main` is the stable branch.
- `nightly` is the active development branch.

All pull requests must target `nightly`.

Changes are merged into `nightly` first and promoted to `main` during releases.

### Practical Guidelines

 - Keep PRs focused and small.
 - One logical change per PR.
 - Write clear commit messages.
 - Rebase before merging.
 - Be open to review feedback.

Discussion is normal. Revisions are normal. Rejection is sometimes necessary.

### Final Notes

Contributions are welcome because open source depends on shared effort — not because every idea must be merged.

If you approach the project with curiosity, technical care, and respect for its direction, you will be welcome here.

[^1]: For smaller, single-purpose libraries. If you need to add a dependency that adds 
      significant functionality, don’t waste time rewriting it, but discuss the 
      addition first. **Never hand-roll any security-critical functionality like encryption!**