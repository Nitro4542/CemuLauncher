<div align="center">

# CemuLauncher

![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/nxtroox/CemuLauncher/total?logo=github&label=Downloads)
![GitHub License](https://img.shields.io/github/license/nxtroox/CemuLauncher?logo=github&label=License)
![GitHub Release](https://img.shields.io/github/v/release/nxtroox/CemuLauncher?logo=github&label=Release)

Automatically installs and updates nightly builds of the Cemu emulator.

</div>

## :cd: Installation

Install the [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

Download the latest version from the [releases page](https://github.com/nxtroox/CemuLauncher/releases) and install it.

Alternatively, you can download a portable from the same page if you'd like.

<details>
<summary>Click here for instructions on how to compile it by yourself.</summary>

### :computer: Compile from source

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).

2. Clone this repository and navigate into it:

    ```bash
    git clone https://github.com/nxtroox/CemuLauncher.git
    cd CemuLauncher
    ```

3. Run the following command to start compiling:

    ```bash
    dotnet publish -c Release -r win-x64
    ```

</details>

## :bulb: Usage

Instead of running Cemu, run CemuLauncher. It will automatically check for updates and install them.

It will create a portable installation of Cemu, so make sure to copy your data to it.

### :hammer_and_wrench: Configuration

> [!IMPORTANT]
> The configuration file is not included by default. You can download it [here](http://github.com/nxtroox/CemuLauncher/blob/main/config.yml).

After installing CemuLauncher, you can configure it using its configuration file located under `%AppData%\CemuLauncher\config.yml`.

The config file should be self-explanatory.

## :scroll: License

This project is licensed under the [MIT License](LICENSE).

This project is not affiliated with Cemu or any other project.
