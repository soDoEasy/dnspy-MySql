﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeatMeterPrePay.QueryBaseView;
using HeatMeterPrePay.TabPage;
using HeatMeterPrePay.Util;

namespace HeatMeterPrePay.QueryTabPage
{
	// Token: 0x02000024 RID: 36
	public class QueryRefundTabPage : UserControl, IQueryAction
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0001219A File Offset: 0x0001039A
		public QueryRefundTabPage()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000121B4 File Offset: 0x000103B4
		private void getDBs()
		{
			DataTable dataTable = this.db.ExecuteQuery("SELECT * FROM userTypeTable");
			DataTable dataTable2 = this.db.ExecuteQuery("SELECT * FROM priceConsistTable");
			if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
			{
				this.userTypeDicts = new List<string>();
				foreach (object obj in dataTable.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					string str = dataRow["typeId"].ToString();
					this.userTypeDicts.Add(str + "-" + dataRow["userType"].ToString());
				}
			}
			if (dataTable2 != null && dataTable2.Rows != null && dataTable2.Rows.Count > 0)
			{
				this.unitPriceDicts = new List<string>();
				foreach (object obj2 in dataTable2.Rows)
				{
					DataRow dataRow2 = (DataRow)obj2;
					string str2 = dataRow2["priceConsistId"].ToString();
					this.unitPriceDicts.Add(str2 + "-" + dataRow2["priceConstistName"].ToString());
				}
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00012344 File Offset: 0x00010544
		public DataTable initDGV(DataTable dt)
		{
			DateTime now = DateTime.Now;
			DateTime d = new DateTime(now.Year, now.Month, now.Day);
			Convert.ToInt64((d - WMConstant.DT1970).TotalSeconds);
			DataTable dataTable = new DataTable();
			dataTable.Columns.AddRange(new DataColumn[]
			{
				new DataColumn("设备号"),
				new DataColumn("用户姓名"),
				new DataColumn("证件号"),
				new DataColumn("联系方式"),
				new DataColumn("地址"),
				new DataColumn("用户类型"),
				new DataColumn("价格类型"),
				new DataColumn("交易类型"),
				new DataColumn("交易量"),
				new DataColumn("交易金额"),
				new DataColumn("交易时间"),
				new DataColumn("操作员")
			});
			if (dt != null)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					DataRow dataRow = dt.Rows[i];
					dataTable.Rows.Add(new object[]
					{
						dataRow["userId"].ToString(),
						dataRow["username"].ToString(),
						dataRow["identityId"].ToString(),
						dataRow["phoneNum"].ToString(),
						dataRow["address"].ToString(),
						dataRow["usertype"].ToString(),
						dataRow["priceType"].ToString(),
						dataRow["paytype"].ToString(),
						ConvertUtils.ToInt32(dataRow["pursuitNum"].ToString()).ToString(),
						dataRow["paynum"].ToString(),
						dataRow["time"].ToString(),
						dataRow["operator"].ToString()
					});
				}
			}
			return dataTable;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x000125AD File Offset: 0x000107AD
		public void moreConditionBtn_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000125AF File Offset: 0x000107AF
		public void queryBtn_Click(object sender, EventArgs e)
		{
			this.queryDB(new Dictionary<string, QueryValue>());
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000125BC File Offset: 0x000107BC
		public void exportExcelBtn_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000125C0 File Offset: 0x000107C0
		public void queryDB(Dictionary<string, QueryValue> dicts)
		{
			if (this.qb == null || dicts == null || dicts.Count < 0)
			{
				return;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("userId");
			dataTable.Columns.Add("username");
			dataTable.Columns.Add("identityId");
			dataTable.Columns.Add("phoneNum");
			dataTable.Columns.Add("address");
			dataTable.Columns.Add("usertype");
			dataTable.Columns.Add("priceType");
			dataTable.Columns.Add("paytype");
			dataTable.Columns.Add("pursuitNum");
			dataTable.Columns.Add("paynum");
			dataTable.Columns.Add("time");
			dataTable.Columns.Add("operator");
			List<string> list = new List<string>();
			string sqlStrForQueryUser = this.getSqlStrForQueryUser(dicts);
			DataTable dataTable2;
			if (sqlStrForQueryUser != "")
			{
				foreach (string key in dicts.Keys)
				{
					QueryValue queryValue = dicts[key];
					this.db.AddParameter(key, queryValue.Value);
				}
				dataTable2 = this.db.ExecuteQuery("SELECT * FROM usersTable WHERE " + sqlStrForQueryUser + " ORDER BY createTime ASC");
				if (dataTable2 != null && dataTable2.Rows != null && dataTable2.Rows.Count > 0)
				{
					foreach (object obj in dataTable2.Rows)
					{
						DataRow dataRow = (DataRow)obj;
						list.Add(dataRow["permanentUserId"].ToString());
					}
				}
			}
			string text = "";
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					string text2 = list[i];
					this.db.AddParameter("permanentUserId" + text2, text2);
					if (i != 0)
					{
						text += " OR ";
					}
					text = text + "permanentUserId=@permanentUserId" + text2;
				}
			}
			else if (list.Count == 0 && dicts.Count != 0 && sqlStrForQueryUser != "")
			{
				this.qb.initDGV(this.initDGV(dataTable));
				return;
			}
			TimeSpan timeSpan = this.qb.getStartDT() - WMConstant.DT1970;
			TimeSpan timeSpan2 = this.qb.getEndDT() - WMConstant.DT1970;
			this.db.AddParameter("createTimeStart", string.Concat(timeSpan.TotalSeconds));
			this.db.AddParameter("createTimeEnd", string.Concat(timeSpan2.TotalSeconds));
			this.db.AddParameter("operateType", "4");
			string sqlKeys = this.getQueryConditionEntitys()[7].SqlKeys;
			if (dicts.ContainsKey(sqlKeys))
			{
				QueryValue queryValue2 = dicts[sqlKeys];
				this.db.AddParameter(sqlKeys, queryValue2.Value);
				string text3 = text;
				text = string.Concat(new string[]
				{
					text3,
					(text == "") ? "" : (" " + queryValue2.AndOr + " "),
					sqlKeys,
					queryValue2.Oper,
					"@",
					sqlKeys
				});
			}
			dataTable2 = this.db.ExecuteQuery("SELECT * FROM userCardLog WHERE " + ((text == "") ? "" : ("(" + text + ") AND ")) + "time>=@createTimeStart AND time<=@createTimeEnd AND operateType=@operateType ORDER BY operationId ASC");
			if (dataTable2 != null && dataTable2.Rows != null && dataTable2.Rows.Count > 0)
			{
				foreach (object obj2 in dataTable2.Rows)
				{
					DataRow dataRow2 = (DataRow)obj2;
					this.db.AddParameter("permanentUserId", dataRow2["permanentUserId"].ToString());
					DataRow dataRow3 = this.db.ExecuteRow("SELECT * FROM usersTable WHERE permanentUserId=@permanentUserId");
					if (dataRow3 != null)
					{
						this.db.AddParameter("typeId", dataRow3["userTypeId"].ToString());
						DataRow dataRow4 = this.db.ExecuteRow("SELECT * FROM userTypeTable WHERE typeId=@typeId");
						this.db.AddParameter("priceConsistId", dataRow3["userPriceConsistId"].ToString());
						DataRow dataRow5 = this.db.ExecuteRow("SELECT * FROM priceConsistTable WHERE priceConsistId=@priceConsistId");
						this.db.AddParameter("userCardLogId", dataRow2["operationId"].ToString());
						DataRow dataRow6 = this.db.ExecuteRow("SELECT * FROM payLogTable WHERE userCardLogId=@userCardLogId");
						if (dataRow6 != null)
						{
							DataRow dataRow7 = dataTable.NewRow();
							dataRow7["userId"] = dataRow3["userId"].ToString();
							dataRow7["username"] = dataRow3["username"].ToString();
							dataRow7["identityId"] = dataRow3["identityId"].ToString();
							dataRow7["phoneNum"] = dataRow3["phoneNum"].ToString();
							dataRow7["address"] = dataRow3["address"].ToString();
							dataRow7["usertype"] = ((dataRow4 == null) ? "" : dataRow4["userType"].ToString());
							dataRow7["priceType"] = ((dataRow5 == null) ? "" : dataRow5["priceConstistName"].ToString());
							dataRow7["paytype"] = WMConstant.UserCardOperateType[(int)(checked((IntPtr)(Convert.ToInt64(dataRow2["operateType"]))))];
							dataRow7["pursuitNum"] = dataRow6["pursuitNum"].ToString();
							dataRow7["paynum"] = dataRow6["realPayNum"].ToString();
							dataRow7["time"] = WMConstant.DT1970.AddSeconds(ConvertUtils.ToDouble(dataRow2["time"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
							dataRow7["operator"] = dataRow2["operator"].ToString();
							dataTable.Rows.Add(dataRow7);
							dataTable.AcceptChanges();
						}
					}
				}
			}
			this.qb.initDGV(this.initDGV(dataTable));
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00012D04 File Offset: 0x00010F04
		public List<QueryConditionEntity> getQueryConditionEntitys()
		{
			List<QueryConditionEntity> list = new List<QueryConditionEntity>();
			QueryConditionEntity item = new QueryConditionEntity("用户姓名", "username", 3, false, null);
			list.Add(item);
			QueryConditionEntity item2 = new QueryConditionEntity("用户地址", "address", 3, false, null);
			list.Add(item2);
			QueryConditionEntity item3 = new QueryConditionEntity("联系方式", "phoneNum", 2, false, null);
			list.Add(item3);
			QueryConditionEntity item4 = new QueryConditionEntity("证件号码", "identityId", 2, false, null);
			list.Add(item4);
			QueryConditionEntity item5 = new QueryConditionEntity("设备号", "userId", 2, false, null);
			list.Add(item5);
			QueryConditionEntity item6 = new QueryConditionEntity("用户类型", "userTypeId", 2, true, this.userTypeDicts);
			list.Add(item6);
			QueryConditionEntity item7 = new QueryConditionEntity("价格类型", "userPriceConsistId", 2, true, this.unitPriceDicts);
			list.Add(item7);
			QueryConditionEntity item8 = new QueryConditionEntity("操作员编号", "operator", 2, false, null);
			list.Add(item8);
			return list;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00012DFC File Offset: 0x00010FFC
		private string getSqlStr(Dictionary<string, QueryValue> sqlDicts)
		{
			string text = "";
			if (sqlDicts == null)
			{
				return text;
			}
			for (int i = 0; i < sqlDicts.Keys.Count; i++)
			{
				string text2 = sqlDicts.Keys.ElementAt(i);
				QueryValue queryValue = sqlDicts[text2];
				string text3 = text;
				text = string.Concat(new string[]
				{
					text3,
					(i == 0) ? "" : (" " + queryValue.AndOr + " "),
					text2,
					" ",
					queryValue.Oper,
					" @",
					text2
				});
			}
			return text;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00012EA8 File Offset: 0x000110A8
		private string getSqlStrForQueryUser(Dictionary<string, QueryValue> sqlDicts)
		{
			string text = "";
			if (sqlDicts == null)
			{
				return text;
			}
			for (int i = 0; i < sqlDicts.Keys.Count; i++)
			{
				string text2 = sqlDicts.Keys.ElementAt(i);
				string sqlKeys = this.getQueryConditionEntitys()[7].SqlKeys;
				if (!(text2 == sqlKeys))
				{
					QueryValue queryValue = sqlDicts[text2];
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						(i == 0) ? "" : (" " + queryValue.AndOr + " "),
						text2,
						" ",
						queryValue.Oper,
						" @",
						text2
					});
				}
			}
			return text;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00012F74 File Offset: 0x00011174
		private void QueryRefundTabPage_Load(object sender, EventArgs e)
		{
			this.qb = new QueryBase();
			this.qb.setQueryAction(this);
			this.qb.Show();
			this.pageContainer.Controls.Clear();
			this.pageContainer.Controls.Add(this.qb);
			this.getDBs();
			this.qb.initDGV(this.initDGV(null));
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00012FE1 File Offset: 0x000111E1
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00013000 File Offset: 0x00011200
		private void InitializeComponent()
		{
			this.pageContainer = new GroupBox();
			this.label19 = new Label();
			base.SuspendLayout();
			this.pageContainer.Location = new Point(9, 51);
			this.pageContainer.Name = "pageContainer";
			this.pageContainer.Size = new Size(682, 516);
			this.pageContainer.TabIndex = 15;
			this.pageContainer.TabStop = false;
			this.label19.AutoSize = true;
			this.label19.Font = new Font("SimSun", 15f, FontStyle.Bold, GraphicsUnit.Point, 134);
			this.label19.Location = new Point(23, 17);
			this.label19.Name = "label19";
			this.label19.Size = new Size(135, 20);
			this.label19.TabIndex = 14;
			this.label19.Text = "退购明细查询";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.pageContainer);
			base.Controls.Add(this.label19);
			base.Name = "QueryRefundTabPage";
			base.Size = new Size(701, 584);
			base.Load += this.QueryRefundTabPage_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000165 RID: 357
		private QueryBase qb;

		// Token: 0x04000166 RID: 358
		private List<string> userTypeDicts;

		// Token: 0x04000167 RID: 359
		private List<string> unitPriceDicts;

		// Token: 0x04000168 RID: 360
		private DbUtil db = new DbUtil();

		// Token: 0x04000169 RID: 361
		private IContainer components;

		// Token: 0x0400016A RID: 362
		private GroupBox pageContainer;

		// Token: 0x0400016B RID: 363
		private Label label19;
	}
}
