﻿/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using System;
using System.Collections.Generic;

namespace extOSC.Serialization.Packers
{
    public class OSCPackerVector4 : OSCPacker<Vector4>
    {
        #region Protected Methods

        protected override Vector4 OSCValuesToValue(List<OSCValue> values, ref int start, Type type)
        {
            var vector = new Vector4
            {
                x = values[start++].FloatValue,
                y = values[start++].FloatValue,
                z = values[start++].FloatValue,
                w = values[start++].FloatValue
            };

            return vector;
        }

        protected override void ValueToOSCValues(List<OSCValue> values, Vector4 value)
        {
            values.Add(OSCValue.Float(value.x));
            values.Add(OSCValue.Float(value.y));
            values.Add(OSCValue.Float(value.z));
            values.Add(OSCValue.Float(value.w));
        }

        #endregion
    }
}