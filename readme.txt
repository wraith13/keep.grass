■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  keep.grass
■

このアプリは github 上の Contributions つまりネット上での俗称である「草」
を途切らせることのないようにスマートフォンで Public Activity の最新の
タイムスタンプとそこから24時間が経過してしまうまでの残り時間を確認する為の
アプリです。


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  アプリストア
■

iOS版
https://itunes.apple.com/us/app/keep.grass/id1170833136?l=ja&ls=1&mt=8

Android版
https://play.google.com/store/apps/details?id=net.trickpalace.keep_grass

UWP版
https://www.microsoft.com/store/apps/9nblggh51p1m


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  構成
■

■.\readme.txt
　→このファイルです。

■.\history.txt
　→リリース用パッケージを作成するバッチファイル。

■.\LICENSE_1_0.txt
　→このソフトウェア本体で採用しているライセンス。
　→このソフトウェアが利用してる各種プラグイン等のライセンスはそれぞれで採用されているライセンスとなります。

■.\source
　→ ソースディレクトリです。

■.\source\keep.grass.sln
　→ Mac版 Xamarin Studio 用のソリューションファイルです。
　　Xamarin Studio では UWP アプリケーションをビルドできないので
　　このソリューションファイルには UWP プロジェクトは含まれていません。

■.\source\keep.grass.uwp.sln
　→ Visual Studio 用のソリューションファイルです。
　　全てのプロジェクトを含みます。

■.\resource
　→ 素材ディレクトリです。

■.\store
　→ ストア提出関連のディレクトリです。


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  利用している Xamarin の Plugin 等
■

■Microsoft HTTP Client Libraries
https://www.nuget.org/packages/Microsoft.Net.Http/

■Xam.Plugins.Settings
https://github.com/jamesmontemagno/SettingsPlugin
https://www.nuget.org/packages/Xam.Plugins.Settings/

■Xam.Plugins.Forms.ImageCircle
https://github.com/jamesmontemagno/ImageCirclePlugin
https://www.nuget.org/packages/Xam.Plugins.Forms.ImageCircle

■NotificationsExtensions.Win10
https://github.com/WindowsNotifications/NotificationsExtensions
https://www.nuget.org/packages/NotificationsExtensions.Win10
→ UWP 版でのみ使用

■SkiaSharp(.Views.Forms)
https://github.com/mono/SkiaSharp
https://www.nuget.org/packages/SkiaSharp.Views.Forms/


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  利用している フォント
■

■Noto Sans CJK jp Regular
https://www.google.com/get/noto/help/cjk/
→グラフ描画内のテキスト為に使用


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  利用している アイコン
■

■GitHub Octicons
https://octicons.github.com
→アクティビティの履歴表示画面にて使用


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  ライセンス
■

Boost Software License - Version 1.0 を採用しています。
詳細は .\LICENSE_1_0.txt を参照してください。

日本語参考訳: http://hamigaki.sourceforge.jp/doc/html/license.html


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  バージョン採番ルール
■

■バージョン表記のフォーマット: A.BB.CCC

■メジャーバージョン番号(A):
明らかな非互換の変更が行われた際にインクリメント。
桁数は不定。

■マイナーバージョン番号(BB):
機能追加や上位互換と判断できる仕様変更が行われた際にインクリメント。
桁数は2桁固定。

■ビルド番号(CCC):
バグフィックスや仕様変更というほどでもない微細な修正が行われた際にインクリ
メント。
桁数は3桁固定。

■細則
・各番号は0始まりとする。
・固定桁に足りない場合は先頭を0埋めする。
・番号が固定桁で足りなくなった場合は、上位の番号をインクリメントする。
・上位の番号がインクリメントされた場合、下位の番号は0にリセットする。


■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
■
■  End Of Document
■
