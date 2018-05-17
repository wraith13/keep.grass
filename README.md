# keep.grass

⚠ 現在、ストアにアップしてるバーションは github の変更に追従していない為、正常稼働しません。また、このリポジトリ上のモノも最新の依存パッケージおよびフレームワークの組み合わせではビルドが通らない問題を抱えています。

このアプリは github 上の Contributions つまりネット上での俗称である「草」を途切らせることのないようにスマートフォンで Public Activity の最新のタイムスタンプとそこから24時間が経過してしまうまでの残り時間を確認する為のアプリです。

|platform|build status|
|-|-|
|iOS|[![Build status](https://build.appcenter.ms/v0.1/apps/af0849f1-9407-463a-b75d-6c5f52b25bb8/branches/master/badge)](https://appcenter.ms)|
|Android|[![Build status](https://build.appcenter.ms/v0.1/apps/6ffce19d-f967-478a-8cfc-e6a8a5fc5a08/branches/master/badge)](https://appcenter.ms)|
|UWP|[![Build status](https://build.appcenter.ms/v0.1/apps/23ea7a36-fb5e-44c7-b68c-96556a5b498a/branches/master/badge)](https://appcenter.ms)|

## アプリストア

* [iOS版](https://itunes.apple.com/us/app/keep.grass/id1170833136?l=ja&ls=1&mt=8)
* [Android版](https://play.google.com/store/apps/details?id=net.trickpalace.keep_grass)
* [UWP版](https://www.microsoft.com/store/apps/9nblggh51p1m)

## 構成

### .\README.md

このファイルです。

### .\history.txt

リリース用パッケージを作成するバッチファイル。

### .\LICENSE_1_0.txt

このソフトウェア本体で採用しているライセンス。
このソフトウェアが利用してる各種プラグイン等のライセンスはそれぞれで採用されているライセンスとなります。

### .\source

ソースディレクトリです。

### .\source\keep.grass.sln

ソリューションファイルです。

### .\resource

素材ディレクトリです。

### .\store

ストア提出関連のディレクトリです。

## 利用している Xamarin の Plugin 等

### Microsoft HTTP Client Libraries

* https://www.nuget.org/packages/Microsoft.Net.Http/

### Xam.Plugins.Settings

* https://github.com/jamesmontemagno/SettingsPlugin
* https://www.nuget.org/packages/Xam.Plugins.Settings/

### Xam.Plugins.Forms.ImageCircle

* https://github.com/jamesmontemagno/ImageCirclePlugin
* https://www.nuget.org/packages/Xam.Plugins.Forms.ImageCircle

### NotificationsExtensions.Win10

* https://github.com/WindowsNotifications/NotificationsExtensions
* https://www.nuget.org/packages/NotificationsExtensions.Win10

> UWP 版でのみ使用

### SkiaSharp(.Views.Forms)

* https://github.com/mono/SkiaSharp
* https://www.nuget.org/packages/SkiaSharp.Views.Forms/

### Json\.NET

* http://www.newtonsoft.com/json
* https://www.nuget.org/packages/Newtonsoft.Json/

## 利用している フォント

### Noto Sans CJK jp Regular

* https://www.google.com/get/noto/help/cjk/

> グラフ描画内のテキスト為に使用

## 利用している アイコン

### GitHub Octicons

* https://octicons.github.com

> アクティビティの履歴表示画面にて使用

## ライセンス

Boost Software License - Version 1.0 を採用しています。
詳細は [.\LICENSE_1_0.txt](./LICENSE_1_0.txt) を参照してください。

日本語参考訳: http://hamigaki.sourceforge.jp/doc/html/license.html

## バージョン採番ルール

### バージョン表記のフォーマット

`A.BB.CCC`

### メジャーバージョン番号(`A`)

明らかな非互換の変更が行われた際にインクリメント。
桁数は不定。

### マイナーバージョン番号(`BB`)

機能追加や上位互換と判断できる仕様変更が行われた際にインクリメント。
桁数は2桁固定。

### ビルド番号(`CCC`)

バグフィックスや仕様変更というほどでもない微細な修正が行われた際にインクリ
メント。
桁数は3桁固定。

### 細則

* 各番号は0始まりとする。
* 固定桁に足りない場合は先頭を0埋めする。
* 番号が固定桁で足りなくなった場合は、上位の番号をインクリメントする。
* 上位の番号がインクリメントされた場合、下位の番号は0にリセットする。
