# NovelSystem
# 操作説明
* ゲーム画面をクリックして進行
* GameObjectにアタッチされたScenarioSequencerのIsAuto項目をtrueにすると自動的にシナリオが進む。
* FileNameにResources内のフォーマットに則ったCSVファイルの名前を入力することでシナリオを切り替えられる…が、デモ用のCSVの用意が一つしかない。
# アピールポイント
## ScenarioSequencer
メインのシーケンスを実行する。
* MainSequence内でCSVに書かれた命令を各実行コンポーネントに送っていく。
* 実行コンポーネントは全てMonoSequentialActorを継承しており、命令の種類を公開しているため、命令の種類ごとに変数を持たずとも、
  リストでまとめて管理して命令の種類が一致するものに命令を送るだけで済む。
  そのため、命令の種類が増えたり、各実行クラス変更があったとしても、本クラスには基本的には影響が無い。
* 複数の非同期関数が全て完了したら完了する非同期関数を用いることで、1フレームで複数の処理を開始し、そのすべてを同時に進行しつつ、全ての完了を待機することができる。
* Activity関数内で利用しているが、WaitCoroutine関数のように、処理完了時にActionを実行する非同期関数を用いることで、複数の非同期関数のいずれかが完了したら完了する非同期関数(授業でWhenAnyという名前で扱っていた)でも、どの関数が完了したのかを検知することができる。
## MonoSequentialActor
各命令を実行するクラスの基底クラス。
* 対応する命令の種類の公開、待機の有無、命令の処理関数の実装を義務付けている
* 処理関数を実行しつつ、そのYieldInstructionと、待機命令の有無を返す関数を公開している。
* 各処理クラスはこれを継承し、それぞれにあった実装をし、それをアタッチし、ScenarioSequencerのMonoSequentialActorのリストに登録することで命令を受けることができる。
  
