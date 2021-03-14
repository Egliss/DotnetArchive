## DotnetArchive

**このパッケージはプレビュー版です。**  
**今後のアップデートにより、いくつかの破壊的変更が加えられる可能性があります。**  

---

|master|NuGet|
|------|-----|
|[![.NET](https://github.com/Egliss/DotnetArchive/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Egliss/DotnetArchive/actions/workflows/dotnet.yml)|[![NuGet Status](https://img.shields.io/nuget/v/DotnetArchive.svg)](https://www.nuget.org/packages/DotnetArchive)|

## 概要

`DotnetArchive`はdotnetがインストールされている環境でファイルのアーカイブ化を行う`dotnet tool`です。  
以下のプラットフォームに対応します。

+ Windows 
+ Mac 
+ Linux

## インストール
[NuGet](https://www.nuget.org/packages/DotnetArchive/) にて配布しています。  
コマンドラインで以下のコマンドを実行して下さい。
```sh
dotnet tool install --global DotnetArchive --version {バージョン}-alpha
```
## フォーマット

+ Zip
+ ~~これから~~


## 使い方
`DotnetArchive`は`archive`コマンドを提供します。  
コマンドは[ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework)を利用して実装されています。  

```sh
$dotnet archive
Usage: DotnetArchive <Command>

Commands:
  zip    generate zip archive.
  help    Display help.
  version    Display version.
```

アーカイブコマンドには`zip`を始めとしたアーカイブフォーマット名ごとにコマンドが用意されています。~~(予定)~~  
各コマンドは`--help`を付与することでコマンドやその引数の説明を確認することができます。  
以下は`zip`コマンドのヘルプ実行例です。  

```sh
$dotnet archive zip --help
Usage: DotnetArchive zip [options...]

generate zip archive.

Options:
  -i, -input <String>             input file or directory (Required)
  -p, -pattern <String>           input directory based glob pattern (Default: *)
  -e, -excludePattern <String>    exclude input directory based glob pattern (Default: *)
  -o, -output <String>            output zip file with extension (Default: output.zip)
  -h, -excludeHidden <Boolean>    exclude hidden file (Default: True)
  -c, -ignoreCase <Boolean>       ignore case (Default: True)
  -q, -quiet                      quiet infomation message (Optional)
```
