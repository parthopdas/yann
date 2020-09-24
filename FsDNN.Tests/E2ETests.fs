﻿module KS.FsDNN.Tests.E2ETests

open KS.FsDNN
open Xunit
open System.Collections.Generic

[<Fact>]
let ``multi-class multi-label classification`` () =
  let n = Net.makeLayers 1 1.0 { N = 7 } [ FullyConnectedLayer {| N = 10; Activation = Sigmoid |}; FullyConnectedLayer {| N = 3; Activation = Linear |} ] (BCEWithLogitsLossLayer {| Classes = 3 |})

  // X, y = sklearn.datasets.make_multilabel_classification(n_samples=50, n_features=7, n_classes=3, n_labels=2, random_state=1)
  let X = [[ 4.;  5.;  2.;  5.;  5.;  4.;  4.;  3.;  4.;  3.;  5.;  6.;  4.;  6.;  7.;  2.;  1.;  5.;
             7.;  3.;  6.;  8.;  5.;  1.;  4.;  2.;  4.;  9.;  1.;  3.;  4.;  2.;  2.;  2.;  2.;  7.;
             0.;  2.;  7.;  3.;  3.;  4.;  4.;  9.;  3.;  3.;  5.;  2.;  4.;  5.;]
           [ 4.;  5.;  2.;  5.; 13.;  6.;  5.;  6.; 11.;  8.;  1.;  4.;  3.;  4.;  4.;  7.;  6.;  4.;
             5.;  4.;  7.;  4.;  6.;  1.;  7.;  5.;  7.;  7.;  2.;  2.;  5.;  4.;  7.;  9.;  2.;  2.;
             0.;  9.;  6.;  6.;  1.;  7.;  7.;  1.;  5.;  8.;  6.;  9.;  3.;  7.;]
           [ 6.;  4.; 14.; 12.;  8.;  6.;  4.;  6.;  6.;  7.; 12.;  9.;  7.;  3.;  8.;  5.;  6.; 13.;
             5.;  7.;  3.;  6.;  8.;  6.;  5.;  5.;  6.; 11.; 12.; 11.;  6.; 10.; 11.;  9.; 13.;  9.;
            10.;  7.; 11.; 13.; 11.; 10.;  6.;  7.;  6.; 10.; 15.;  8.;  5.;  8.;]
           [ 6.; 13.;  5.;  2.;  4.; 10.; 11.; 14.; 15.;  3.;  6.; 17.;  7.;  2.; 12.; 10.; 21.; 19.;
             2.;  3.;  8.;  9.;  3.; 10.;  7.;  3.;  5.;  7.;  8.;  4.; 15.;  5.;  4.;  9.;  6.; 16.;
             3.;  5.;  4.;  6.;  9.; 18.;  9.;  2.;  9.; 20.;  8.;  6.;  4.; 16.;]
           [10.;  9.; 14.; 14.;  6.;  6.;  4.;  2.;  7.; 13.;  7.;  8.;  8.;  5.;  7.; 13.; 11.; 10.;
             6.; 10.;  9.;  6.; 10.;  6.; 11.;  4.;  3.;  8.;  8.;  7.;  7.; 10.;  7.;  6.; 12.; 14.;
             6.;  8.; 11.; 10.;  6.;  3.;  5.; 11.; 12.;  7.; 15.; 10.;  5.;  6.;]
           [ 2.;  1.;  4.;  8.; 11.;  4.;  4.;  8.;  7.;  7.;  8.;  2.;  6.;  6.;  0.;  0.;  6.;  7.;
            11.;  1.;  9.;  5.;  6.;  6.;  4.;  7.;  6.;  3.; 10.;  1.;  4.;  4.;  7.;  6.;  7.;  7.;
             4.;  3.;  4.;  1.;  4.;  1.;  6.;  1.;  2.;  6.;  5.;  2.;  4.;  4.;]
           [ 7.; 11.; 12.;  9.; 15.; 16.; 18.;  7.;  7.;  8.; 17.; 11.; 11.;  8.;  8.; 10.; 10.;  6.;
             2.; 12.;  7.; 11.;  4.;  7.; 13.; 11.;  7.;  7.; 14.; 17.;  6.; 12.;  7.; 11.;  4.;  9.;
            16.; 11.; 13.; 12.;  9.; 11.;  6.; 21.;  7.;  3.;  9.; 11.;  7.;  1.;]] |> Tensor.ofListOfList

  let Y = [[1.; 0.; 1.; 1.; 1.; 1.; 1.; 0.; 0.; 1.; 1.; 1.; 1.; 0.; 1.; 1.; 0.; 0.; 0.; 1.; 0.; 0.; 0.; 1.; 1.; 1.; 0.; 0.; 1.; 1.; 0.; 1.; 1.; 1.; 1.; 1.
            1.; 1.; 1.; 1.; 1.; 0.; 0.; 1.; 1.; 0.; 1.; 1.; 0.; 0.]
           [1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 1.; 0.; 1.; 1.; 0.; 1.; 1.; 1.; 1.; 0.; 0.; 0.; 0.; 0.; 1.; 1.; 1.; 0.; 0.; 1.; 0.; 1.; 1.; 1.; 1.; 1.; 1.
            0.; 1.; 0.; 1.; 1.; 1.; 0.; 0.; 1.; 1.; 1.; 1.; 0.; 1.]
           [0.; 0.; 1.; 1.; 1.; 0.; 0.; 0.; 0.; 1.; 0.; 0.; 1.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 0.; 1.; 0.; 1.; 0.; 0.; 1.; 0.; 0.; 1.; 1.; 1.; 1.; 0.
            0.; 1.; 0.; 0.; 1.; 0.; 0.; 0.; 0.; 0.; 0.; 1.; 0.; 0.]]

  let costs = Dictionary<int, Tensor<double>>()
  let cb = fun e _ J -> if e % 1 = 0 then costs.[e] <- J else ()
  let hp = { HyperParameters.Defaults with Epochs = 3000; LearningRate = TensorR0 0.5 }

  let n = Trainer.trainWithGD cb n X (Y |> Tensor.ofListOfList) hp

  let Y' = X |> Net.predict n

  Y' |> shouldBeEquivalentToWithPrecision 4e-3 Y