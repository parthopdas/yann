﻿namespace KS.FsDNN

module NullRegularizer =
  let regularizeCost _ = id

  let regularizeGradients _ = id

module L2Regularizer =

  let regularizeCost (lambda: double) m (parameters: Parameters) (cost: Tensor<double>): Tensor<double> =
    let _folder acc (k: string) (v: Tensor<double>) =
      if (k.[0] = 'W') then
        acc + v.PointwisePower(2.).ColumnSums().Sum()
      else
        acc

    let sos = parameters |> Map.fold _folder 0.
    let l2RegCost = lambda / (2. * double m) * sos

    cost.PointwiseAdd(l2RegCost)

  let regularizeGradients (lambda: double) m (parameters: Parameters) (gradients: Gradients): Gradients =
    let _mapper lambda (k: string) (v: Tensor<double>) =
      if (k.[0] = 'W') then
        v + (lambda / double m) * parameters.[k]
      else
        v

    gradients |> Map.map (_mapper lambda)

[<AutoOpen>]
module RegularizerDomain =
  type Regularizer =
    | NullRegularizer
    | L2Regularizer of double

  type RegularizerDefinition =
    { RegularizeCost: Parameters -> Tensor<double> -> Tensor<double>
      RegularizeGradients: Parameters -> Gradients -> Gradients }

module Regularizer =

  let get m = function
    | NullRegularizer -> { RegularizeCost = NullRegularizer.regularizeCost; RegularizeGradients = NullRegularizer.regularizeGradients  }
    | L2Regularizer lambda -> { RegularizeCost = L2Regularizer.regularizeCost lambda m; RegularizeGradients = L2Regularizer.regularizeGradients lambda m }
