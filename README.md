# NovelSystem
## ScenarioSequencer
メインのシーケンスを実行する。
* MainSequence内でCSVに書かれた命令を各実行コンポーネントに送っていく。
* 実行コンポーネントは全てMonoSequentialActorを継承しており、命令の種類を公開しているため、命令の種類ごとに変数を持たずとも、
  リストでまとめて管理して命令の種類が一致するものに命令を送るだけで済む。
  そのため、命令の種類が増えたり、各実行クラス変更があったとしても、本クラスには基本的には影響が無い。
## MonoSequentialActor
