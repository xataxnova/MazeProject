using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharpCodeGen {

    public class SharpFormator
    {
        public struct IndentItem
        {
            public int indent_val;
            public SharpElement element;
        }

        public int current_indent;

        private Stack<int> right_brace_stack = new Stack<int>();

        public void Reset()
        {
            current_indent = 0;
        }

        public void PushBrace()
        {
            int val = current_indent;
            right_brace_stack.Push(val);
            current_indent++;
        }

        public int PopBrace()
        {
            int val = right_brace_stack.Pop();
            current_indent--;
            return val;
        }
    }
}