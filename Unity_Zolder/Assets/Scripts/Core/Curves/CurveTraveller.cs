// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	public class CurveTraveller : MonoBehaviour
	{
		[SerializeField] private CompoundCurve curve;
		// speed meters per second
		[SerializeField] private float _speed;
		public float speed { get { return _speed; } set { _speed = value; } }
		// a range between 5 and 25 is sane for precisionIterations. lower is faster but less precise.
		[SerializeField] private int _precisionIterations = 12;
		public int precisionIterations { get { return _precisionIterations; } set { _precisionIterations = value; } }
		public float curveTime { get; private set; }
		private Transform _trans;
		private Transform trans
		{
			get
			{
				if (_trans == null)
				{
					_trans = transform;
				}

				return _trans;
			}
		}

		void Update()
		{
			if (curve != null)
			{
				curveTime += Time.deltaTime * speed / curve.Length;
				float reparam = curve.Reparameterize(curveTime, precisionIterations);

				trans.position = curve.Evaluate(reparam);
				trans.forward = curve.Derivative(reparam);
			}
		}
	}
}