# Synthesis

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/jimmy-mll/Synthesis)](https://github.com/jimmy-mll/Synthesis/issues)
[![GitHub stars](https://img.shields.io/github/stars/jimmy-mll/Synthesis)](https://github.com/jimmy-mll/Synthesis/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/jimmy-mll/Synthesis)](https://github.com/jimmy-mll/Synthesis/network)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/jimmy-mll/Synthesis)](https://github.com/jimmy-mll/Synthesis/pulls)
[![GitHub contributors](https://img.shields.io/github/contributors/jimmy-mll/Synthesis)](https://github.com/jimmy-mll/Synthesis/graphs/contributors)

Welcome to the Synthesis project, aiming to recreate the classic Dofus game experience from version 2.12, while maintaining a professional and clean codebase.

## Table of Contents

- [Introduction](#introduction)
- [Getting Started](#getting-started)
- [Features](#features)
- [Changelog](#changelog)
- [Contribution](#contribution)
- [License](#license)

## Introduction

Dofus 2.12 Emulator is a community-driven project focused on emulating the beloved Dofus 2.12 version. Our mission is to provide a nostalgic and authentic experience while ensuring the highest quality and professionalism in code development.

## Getting Started

### Prerequisites

Before you begin, ensure you have met the following requirements:

- [Rider](https://www.jetbrains.com/rider)
- [.NET8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download)
- [Git](https://git-scm.com/downloads)

### Installation

1. Clone this repository:

```sh
git clone https://github.com/jimmy-mll/Synthesis.git
cd Synthesis
```

2. Configure your configuration variables in `appsettings.json`.

3. Run the project:

```sh
dotnet run src/servers/Synthesis.Servers.AuthServer.csproj
dotnet run src/servers/Synthesis.Servers.GameServer.csproj
```

## Changelog

For a detailed list of changes in this project, please refer to the [CHANGELOG](CHANGELOG.md).

## Contribution

We welcome contributions from the community to help improve and expand the emulator. To contribute, follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit your changes using Conventional Commits (see [CONTRIBUTING](CONTRIBUTING.md)).
4. Create a pull request with a clear description of your changes.

Please review our [CONTRIBUTING](CONTRIBUTING.md) for more details on our contribution guidelines.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Dofus is a registered trademark of Ankama. This project is not affiliated with or endorsed by Ankama.