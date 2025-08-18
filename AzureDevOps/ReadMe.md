# Azure DevOps Automation

CI/CD pipeline configurations and automation tools for Azure DevOps.

## TL;DR - Quick Start

This folder contains Azure DevOps pipeline configurations that help automate common development tasks:

- **auto-update-nuget-packages.az-pipelines.yml** - Automatically creates PRs to update outdated NuGet packages

Simply add the pipeline file to your Azure DevOps project and configure it to run on a schedule to keep your dependencies up to date.

## Deep Dive

### Auto-Update NuGet Packages Pipeline

A pipeline that uses `dotnet-outdated` to continuously create pull requests that update outdated NuGet packages in your solution.

#### Features
- Automatically scans for outdated NuGet packages
- Creates pull requests with package updates
- Can be scheduled to run regularly (e.g., weekly)
- Helps keep dependencies current and secure

#### Setup
1. Add the `auto-update-nuget-packages.az-pipelines.yml` file to your repository
2. Create a new pipeline in Azure DevOps pointing to this YAML file
3. Configure appropriate triggers (e.g., scheduled runs)
4. Ensure the pipeline has permissions to create pull requests

#### Configuration
The pipeline can be customized to:
- Target specific projects or solutions
- Exclude certain packages from updates
- Adjust update frequency
- Configure notification settings
