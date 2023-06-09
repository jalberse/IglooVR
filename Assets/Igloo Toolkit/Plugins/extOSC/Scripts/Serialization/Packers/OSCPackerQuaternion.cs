﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerQuaternion : OSCPacker<Quaternion>
    {
        #region Protected Methods

        protected override Quaternion OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var quaternion = new Quaternion
            {
                x = values[start++].FloatValue,
                y = values[start++].FloatValue,
                z = values[start++].FloatValue,
                w = values[start++].FloatValue
            };

            return quaternion;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, Quaternion value)
        {
            values.Add(OSCValue.Float(value.x));
            values.Add(OSCValue.Float(value.y));
            values.Add(OSCValue.Float(value.z));
            values.Add(OSCValue.Float(value.w));
        }

        #endregion
    }
}